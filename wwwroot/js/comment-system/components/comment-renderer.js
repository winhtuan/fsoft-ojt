// ============================================================
// COMMENT RENDERER - Xử lý việc render bình luận
// ============================================================

import { timeAgo } from "../../helpers/time.js";

export class CommentRenderer {
  constructor(config, elements) {
    this.config = config;
    this.elements = elements;
  }

  createCommentNode() {
    return this.elements.commentTemplate.content
      .cloneNode(true)
      .querySelector(".comment-item");
  }

  isCommentOwner(comment) {
    return (
      String(this.config.settings.currentUserId) === String(comment.userId)
    );
  }

  renderBasicInfo(commentNode, comment) {
    const avatarImg = commentNode.querySelector(".comment-avatar img");
    avatarImg.src = comment.avatarUrl?.trim() || "/img/default-avatar.png";
    avatarImg.onerror = () => (avatarImg.src = "/img/default-avatar.png");

    commentNode.querySelector(".comment-author").textContent =
      comment.userName || "Người dùng ẩn danh";
    commentNode.querySelector(".comment-date").textContent = timeAgo(
      new Date(comment.createdAt)
    );
    commentNode.querySelector(".comment-text").textContent = comment.content;

    // Ẩn dropdown nếu không phải chủ sở hữu
    const isOwner = this.isCommentOwner(comment);
    const optionsDropdown = commentNode.querySelector(
      ".comment-options-dropdown"
    );
    if (!isOwner && optionsDropdown) {
      optionsDropdown.style.display = "none";
    }
  }

  renderReplies(commentNode, comment, renderCommentCallback) {
    const replyListContainer = commentNode.querySelector(
      ".reply-list-container"
    );
    const hasReplies = comment.replies?.length > 0;

    if (!hasReplies) return;

    comment.replies.forEach((reply) => {
      renderCommentCallback(reply, replyListContainer);
    });

    if (comment.parentCommentId == null) {
      this.addToggleRepliesButton(commentNode, comment, replyListContainer);
    }
  }

  addToggleRepliesButton(commentNode, comment, replyListContainer) {
    const commentActions = commentNode.querySelector(".comment-actions");
    const toggleBtn = document.createElement("a");

    toggleBtn.href = "#";
    toggleBtn.className =
      "small text-decoration-none ms-2 toggle-replies-btn fw-bold";
    toggleBtn.textContent = `Xem ${comment.replies.length} phản hồi`;

    let repliesVisible = false;
    replyListContainer.style.display = "none";

    toggleBtn.addEventListener("click", (e) => {
      e.preventDefault();
      repliesVisible = !repliesVisible;
      replyListContainer.style.display = repliesVisible ? "block" : "none";
      toggleBtn.textContent = repliesVisible
        ? "Ẩn phản hồi"
        : `Xem ${comment.replies.length} phản hồi`;
    });

    commentActions.appendChild(toggleBtn);
  }

  appendToDOM(commentNode, parentNode, prepend) {
    if (parentNode) {
      prepend
        ? parentNode.prepend(commentNode)
        : parentNode.appendChild(commentNode);
    } else {
      if (prepend) {
        this.elements.commentList.prepend(commentNode);
        commentNode.classList.add("new-comment");
        setTimeout(() => commentNode.classList.remove("new-comment"), 1500);
      } else {
        this.elements.commentList.appendChild(commentNode);
      }
    }
  }
}
