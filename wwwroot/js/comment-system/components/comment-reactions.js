// ============================================================
// COMMENT REACTIONS - Xử lý reaction (like/unlike)
// ============================================================

import { notify } from "../helpers/notify.js";

export class CommentReactions {
  constructor(config, api) {
    this.config = config;
    this.api = api;
  }

  attachReactionHandler(commentNode, comment) {
    const reactButton = commentNode.querySelector(".btn-react");
    const reactCountSpan = commentNode.querySelector(".react-count");
    if (!reactButton || !reactCountSpan) return;

    reactCountSpan.textContent = comment.reactCount || 0;
    if (comment.isReactedByCurrentUser) {
      reactButton.classList.add("active");
    }

    reactButton.addEventListener("click", async (e) => {
      e.preventDefault();

      // Kiểm tra đăng nhập trước khi react
      if (!this.config.settings.isAuthenticated) {
        notify("Bạn cần đăng nhập để thích bình luận.", "Cần đăng nhập");
        return;
      }

      await this.handleReaction(reactButton, reactCountSpan, comment.commentId);
    });
  }

  async handleReaction(reactButton, reactCountSpan, commentId) {
    const wasReacted = reactButton.classList.contains("active");
    const oldReactCount = parseInt(reactCountSpan.textContent);

    // Cập nhật UI ngay lập tức
    reactButton.classList.toggle("active");
    reactCountSpan.textContent = wasReacted
      ? oldReactCount - 1
      : oldReactCount + 1;

    try {
      const result = await this.api.reactComment(commentId, !wasReacted);
      if (result.success) {
        reactCountSpan.textContent = result.data.reactCount;
      } else {
        throw new Error(result.message);
      }
    } catch (error) {
      // Khôi phục lại nếu lỗi
      reactButton.classList.toggle("active");
      reactCountSpan.textContent = oldReactCount;

      const msg = error.message || "Lỗi khi thực hiện hành động.";
      notify(msg, "Lỗi");
    }
  }
}
