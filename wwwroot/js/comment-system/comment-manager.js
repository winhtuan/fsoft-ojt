// ============================================================
// COMMENT MANAGER - Main orchestrator class
// ============================================================

import { notify } from "../helpers/notify.js";
import { CommentConfig } from "./config.js";
import { CommentAPI } from "./services/comment-api.js";
import { CommentRenderer } from "./components/comment-renderer.js";
import { CommentReactions } from "./components/comment-reactions.js";
import { CommentActions } from "./components/comment-actions.js";

export class CommentManager {
  constructor() {
    this.config = new CommentConfig();

    if (!this.config.isValid()) {
      console.error("Không tìm thấy khu vực bình luận.");
      return;
    }

    this.api = new CommentAPI(this.config.settings);
    this.renderer = new CommentRenderer(this.config, this.config.elements);
    this.reactions = new CommentReactions(this.config, this.api);
    this.actions = new CommentActions(this.config, this.api);

    this.init();
  }

  init() {
    this.attachEventListeners();
    this.loadComments();
  }

  attachEventListeners() {
    const { btnSubmitComment } = this.config.elements;
    if (btnSubmitComment) {
      btnSubmitComment.addEventListener("click", (e) => {
        e.preventDefault();
        this.handleSubmitComment();
      });
    }
  }

  // ============================================================
  // LOAD COMMENTS
  // ============================================================

  async loadComments() {
    const { commentList } = this.config.elements;

    try {
      const result = await this.api.loadComments(this.config.settings.plantId);

      if (result.success && result.data) {
        commentList.innerHTML = "";

        if (result.data.length === 0) {
          commentList.innerHTML =
            '<p class="text-muted">Chưa có bình luận nào. Hãy là người đầu tiên!</p>';
        } else {
          result.data.forEach((comment) => this.renderComment(comment));
        }
      } else {
        this.showError("Đã xảy ra lỗi khi tải bình luận.", commentList);
      }
    } catch (error) {
      console.error("Lỗi khi tải bình luận:", error);
      this.showError("Đã xảy ra lỗi kết nối. Vui lòng thử lại.", commentList);
    }
  }

  // ============================================================
  // SUBMIT NEW COMMENT
  // ============================================================

  async handleSubmitComment() {
    if (!this.config.settings.isAuthenticated) {
      this.showFormError("Bạn cần đăng nhập để bình luận.");
      notify("Bạn cần đăng nhập để bình luận.", "Cần đăng nhập");
      return;
    }

    const { commentContent, commentError, btnSubmitComment, commentList } =
      this.config.elements;
    const content = commentContent.value.trim();

    if (!content) {
      this.showFormError("Vui lòng nhập nội dung bình luận.");
      notify("Vui lòng nhập nội dung bình luận.", "Thiếu nội dung");
      return;
    }

    commentError.classList.add("d-none");
    btnSubmitComment.disabled = true;

    try {
      const result = await this.api.createComment(content);
      if (result.success) {
        this.renderComment(result.data, null, true);
        commentContent.value = "";
        commentList.querySelector("p.text-muted")?.remove();
      } else {
        this.showFormError(
          result.message || "Không thể gửi bình luận. Vui lòng thử lại."
        );
        notify(
          result.message || "Không thể gửi bình luận. Vui lòng thử lại.",
          "Lỗi"
        );
      }
    } catch (error) {
      console.error("Lỗi khi gửi bình luận:", error);
      let errorMessage = "Đã xảy ra sự cố không mong muốn. Vui lòng thử lại.";
      if (error && error.message) {
        const messageParts = error.message.split("Error: ");
        errorMessage =
          messageParts.length > 1 ? messageParts[1] : error.message;
      }
      notify(errorMessage, "Lỗi");
    } finally {
      btnSubmitComment.disabled = false;
    }
  }

  // ============================================================
  // RENDER COMMENT (Main method)
  // ============================================================

  renderComment(comment, parentNode = null, prepend = false) {
    const commentNode = this.renderer.createCommentNode();
    const isOwner = this.renderer.isCommentOwner(comment);

    // Render basic info
    this.renderer.renderBasicInfo(commentNode, comment);

    // Attach reaction handler
    this.reactions.attachReactionHandler(commentNode, comment);

    // Attach owner actions (edit, delete)
    if (isOwner) {
      this.actions.attachOwnerActions(
        commentNode,
        comment,
        this.reRenderComment.bind(this)
      );
    }

    // Attach reply handler
    this.actions.attachReplyHandler(
      commentNode,
      comment,
      this.renderComment.bind(this)
    );

    // Render replies
    this.renderer.renderReplies(
      commentNode,
      comment,
      this.renderComment.bind(this)
    );

    // Append to DOM
    this.renderer.appendToDOM(commentNode, parentNode, prepend);
  }

  // ============================================================
  // HELPER METHODS
  // ============================================================

  reRenderComment(commentNode, comment) {
    const tempDiv = document.createElement("div");
    this.renderComment(comment, tempDiv);
    commentNode.replaceWith(tempDiv.firstChild);
  }

  showError(message, container) {
    container.innerHTML = `<p class="text-danger">${message}</p>`;
    notify(message, "Lỗi");
  }

  showFormError(message) {
    const { commentError } = this.config.elements;
    if (commentError) {
      commentError.textContent = message;
      commentError.classList.remove("d-none");
    }
  }
}
