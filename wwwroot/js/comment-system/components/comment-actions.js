// ============================================================
// COMMENT ACTIONS - Xử lý edit, delete, reply
// ============================================================

import { notify, confirmAction } from "../../helpers/notify.js";

export class CommentActions {
  constructor(config, api) {
    this.config = config;
    this.api = api;
  }

  // ============================================================
  // CHỈNH SỬA & XÓA
  // ============================================================

  attachOwnerActions(commentNode, comment, reRenderCallback) {
    const optionsDropdown = commentNode.querySelector(
      ".comment-options-dropdown"
    );
    if (!optionsDropdown) return;
    optionsDropdown.style.display = "block";

    // Tăng z-index khi dropdown mở
    const dropdownToggle = optionsDropdown.querySelector(
      '[data-bs-toggle="dropdown"]'
    );
    if (dropdownToggle) {
      dropdownToggle.addEventListener("show.bs.dropdown", () => {
        commentNode.style.zIndex = "1000";
      });

      dropdownToggle.addEventListener("hide.bs.dropdown", () => {
        commentNode.style.zIndex = "";
      });
    }

    const btnEdit = optionsDropdown.querySelector(".btn-edit");
    const btnDelete = optionsDropdown.querySelector(".btn-delete");

    if (btnEdit) {
      btnEdit.addEventListener("click", (e) => {
        e.preventDefault();
        this.handleEdit(commentNode, comment, reRenderCallback);
      });
    }

    if (btnDelete) {
      btnDelete.addEventListener("click", (e) => {
        e.preventDefault();
        this.handleDelete(commentNode, comment);
      });
    }
  }

  handleEdit(commentNode, comment, reRenderCallback) {
    const commentBubble = commentNode.querySelector(".comment-bubble");
    const commentTextNode = commentNode.querySelector(".comment-text");
    const originalContent = commentTextNode.textContent;

    commentBubble.innerHTML = `
      <div class="edit-form p-2">
        <textarea class="form-control form-control-sm mb-2">${originalContent}</textarea>
        <button class="btn btn-success btn-sm btn-save-edit">Lưu</button>
        <button class="btn btn-secondary btn-sm btn-cancel-edit">Hủy</button>
        <small class="text-danger ms-2 edit-error"></small>
      </div>`;

    const editTextArea = commentBubble.querySelector("textarea");
    editTextArea.focus();

    commentBubble
      .querySelector(".btn-cancel-edit")
      .addEventListener("click", () => {
        reRenderCallback(commentNode, comment);
      });

    commentBubble
      .querySelector(".btn-save-edit")
      .addEventListener("click", async () => {
        await this.saveEdit(
          commentNode,
          comment,
          editTextArea,
          originalContent,
          reRenderCallback
        );
      });
  }

  async saveEdit(
    commentNode,
    comment,
    editTextArea,
    originalContent,
    reRenderCallback
  ) {
    const newContent = editTextArea.value.trim();

    if (!newContent || newContent === originalContent) {
      reRenderCallback(commentNode, comment);
      return;
    }

    try {
      await this.api.updateComment(comment.commentId, newContent);
      comment.content = newContent;
      reRenderCallback(commentNode, comment);
    } catch (error) {
      const errorElement = commentNode.querySelector(".edit-error");
      errorElement.textContent = error.message || "Lỗi. Vui lòng thử lại.";
      notify(error.message || "Lỗi. Vui lòng thử lại.", "Lỗi");
    }
  }

  async handleDelete(commentNode, comment) {
    const doDelete = async () => {
      try {
        await this.api.deleteComment(comment.commentId);

        commentNode.style.transition = "opacity 0.5s ease";
        commentNode.style.opacity = "0";
        setTimeout(() => commentNode.remove(), 500);
        notify("Đã xóa bình luận.", "Hoàn tất");
      } catch (err) {
        const msg = err.message || "Không thể xóa bình luận. Vui lòng thử lại.";
        notify(msg, "Lỗi");
      }
    };

    confirmAction(
      "Bạn có chắc chắn muốn xóa bình luận này?",
      async () => await doDelete(),
      {}
    );
  }

  // ============================================================
  // TRẢ LỜI
  // ============================================================

  attachReplyHandler(commentNode, comment, renderCommentCallback) {
    const replyButton = commentNode.querySelector(".btn-reply");
    if (!replyButton) return;

    const dynamicFormContainer = commentNode.querySelector(
      ".dynamic-form-container"
    );

    replyButton.addEventListener("click", (e) => {
      e.preventDefault();

      // Kiểm tra đăng nhập trước khi reply
      if (!this.config.settings.isAuthenticated) {
        notify("Bạn cần đăng nhập để trả lời bình luận.", "Cần đăng nhập");
        return;
      }

      this.toggleReplyForm(
        dynamicFormContainer,
        comment,
        renderCommentCallback
      );
    });
  }

  toggleReplyForm(container, comment, renderCommentCallback) {
    const existingForm = container.querySelector(".reply-form");

    if (existingForm) {
      existingForm.remove();
      return;
    }

    container.innerHTML = `
      <div class="reply-form mt-2">
        <textarea class="form-control mb-1 reply-content" rows="2" placeholder="Phản hồi tới ${comment.userName}..."></textarea>
        <div class="d-flex align-items-center">
          <button class="btn btn-success btn-sm btn-submit-reply">Gửi</button>
          <button class="btn btn-secondary btn-sm ms-2 btn-cancel-reply">Hủy</button>
          <small class="text-danger ms-2 reply-error"></small>
        </div>
      </div>`;

    const replyForm = container.querySelector(".reply-form");
    replyForm.querySelector(".reply-content").focus();

    replyForm
      .querySelector(".btn-cancel-reply")
      .addEventListener("click", () => {
        replyForm.remove();
      });

    replyForm
      .querySelector(".btn-submit-reply")
      .addEventListener("click", async () => {
        await this.submitReply(replyForm, comment, renderCommentCallback);
      });
  }

  async submitReply(replyForm, comment, renderCommentCallback) {
    const content = replyForm.querySelector(".reply-content").value.trim();
    if (!content) {
      const errorElement = replyForm.querySelector(".reply-error");
      errorElement.textContent = "Vui lòng nhập nội dung phản hồi.";
      notify("Vui lòng nhập nội dung phản hồi.", "Thiếu nội dung");
      return;
    }

    try {
      const result = await this.api.createComment(content, comment.commentId);

      if (result.success) {
        const commentNode = replyForm.closest(".comment-item");
        const replyListContainer = commentNode.querySelector(
          ".reply-list-container"
        );
        renderCommentCallback(result.data, replyListContainer, true);
        replyForm.remove();
      } else {
        throw new Error(result.message);
      }
    } catch (error) {
      const errorElement = replyForm.querySelector(".reply-error");
      errorElement.textContent = error.message || "Lỗi. Thử lại.";
      notify(error.message || "Lỗi. Thử lại.", "Lỗi");
    }
  }
}
