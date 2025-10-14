// ============================================================
// NOTIFY HELPERS - Luôn hiển thị bằng popup (modalHelper)
// ============================================================
function notify(message, title = "Thông báo") {
  if (
    window.modalHelper &&
    typeof window.modalHelper.showAlert === "function"
  ) {
    return window.modalHelper.showAlert(message, title);
  }
  // Bắt buộc hiển thị bằng popup: nếu thiếu modalHelper thì log lỗi
  console.error("[ModalHelper Missing] ", title, message);
}

function confirmAction(message, onOk, options) {
  if (
    window.modalHelper &&
    typeof window.modalHelper.showConfirm === "function"
  ) {
    return window.modalHelper.showConfirm(message, onOk, options);
  }
  console.error("[ModalHelper Missing] Xác nhận:", message);
}

// ============================================================
// COMMENT MANAGER - Quản lý hệ thống bình luận (Modal-based)
// ============================================================

class CommentManager {
  constructor() {
    this.elements = this.initializeElements();
    this.config = this.initializeConfig();

    if (!this.isValid()) {
      console.error("Không tìm thấy khu vực bình luận.");
      return;
    }

    this.init();
  }

  // Khởi tạo các elements DOM
  initializeElements() {
    return {
      commentSection: document.querySelector("section[data-plant-id]"),
      commentList: document.getElementById("commentList"),
      commentTemplate: document.getElementById("commentTemplate"),
      btnSubmitComment: document.getElementById("btnSubmitComment"),
      commentContent: document.getElementById("commentContent"),
      commentError: document.getElementById("commentError"),
    };
  }

  // Khởi tạo cấu hình
  initializeConfig() {
    const { commentSection } = this.elements;
    return {
      currentUserId: window.currentUserId,
      plantId: commentSection?.dataset.plantId,
      antiForgeryToken: document.querySelector(
        'input[name="__RequestVerificationToken"]'
      )?.value,
      isAuthenticated:
        window.currentUserId != null &&
        window.currentUserId !== "" &&
        window.currentUserId !== undefined,
    };
  }

  // Kiểm tra tính hợp lệ
  isValid() {
    return this.elements.commentSection !== null;
  }

  // Khởi chạy
  init() {
    this.attachEventListeners();
    this.loadComments();
  }

  // Gắn sự kiện
  attachEventListeners() {
    if (this.elements.btnSubmitComment) {
      this.elements.btnSubmitComment.addEventListener("click", (e) => {
        e.preventDefault();
        this.handleSubmitComment();
      });
    }
  }

  // ============================================================
  // XỬ LÝ BÌNH LUẬN
  // ============================================================

  async loadComments() {
    const { commentList } = this.elements;

    try {
      const response = await fetch(`/api/Comment/${this.config.plantId}`);
      if (!response.ok) throw new Error("Không thể tải bình luận.");

      const result = await response.json();

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

  async handleSubmitComment() {
    // Kiểm tra đăng nhập
    if (!this.config.isAuthenticated) {
      this.showFormError("Bạn cần đăng nhập để bình luận.");
      notify("Bạn cần đăng nhập để bình luận.", "Cần đăng nhập");
      return;
    }

    const { commentContent, commentError, btnSubmitComment, commentList } =
      this.elements;
    const content = commentContent.value.trim();

    if (!content) {
      this.showFormError("Vui lòng nhập nội dung bình luận.");
      notify("Vui lòng nhập nội dung bình luận.", "Thiếu nội dung");
      return;
    }

    commentError.classList.add("d-none");
    btnSubmitComment.disabled = true;

    try {
      const result = await this.apiCreateComment(content);

      if (result.success) {
        this.renderComment(result.data, null, true);
        commentContent.value = "";
        commentList.querySelector("p.text-muted")?.remove();
        notify("Đã gửi bình luận.", "Thành công");
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
      this.showFormError("Đã xảy ra lỗi kết nối. Vui lòng thử lại.");
      notify("Đã xảy ra lỗi kết nối. Vui lòng thử lại.", "Lỗi");
    } finally {
      btnSubmitComment.disabled = false;
    }
  }

  // ============================================================
  // RENDER BÌNH LUẬN
  // ============================================================

  renderComment(comment, parentNode = null, prepend = false) {
    const commentNode = this.createCommentNode(comment);
    const isOwner = this.isCommentOwner(comment);

    this.renderBasicInfo(commentNode, comment);
    this.attachReactionHandler(commentNode, comment);

    // Xử lý dropdown và reply
    if (isOwner) {
      this.attachOwnerActions(commentNode, comment);
    }

    // Luôn attach reply handler (cho phép tự trả lời chính mình)
    this.attachReplyHandler(commentNode, comment);

    this.renderReplies(commentNode, comment);
    this.appendToDOM(commentNode, parentNode, prepend);
  }

  createCommentNode(comment) {
    return this.elements.commentTemplate.content
      .cloneNode(true)
      .querySelector(".comment-item");
  }

  isCommentOwner(comment) {
    return String(this.config.currentUserId) === String(comment.userId);
  }

  renderBasicInfo(commentNode, comment) {
    const avatarImg = commentNode.querySelector(".comment-avatar img");
    avatarImg.src = comment.avatarUrl?.trim() || "/img/default-avatar.png";
    avatarImg.onerror = () => (avatarImg.src = "/img/default-avatar.png");

    commentNode.querySelector(".comment-author").textContent =
      comment.userName || "Người dùng ẩn danh";
    commentNode.querySelector(".comment-date").textContent = this.timeAgo(
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

  // ============================================================
  // XỬ LÝ REACTION (LIKE)
  // ============================================================

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
      if (!this.config.isAuthenticated) {
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
      const result = await this.apiReactComment(commentId, !wasReacted);
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

  // ============================================================
  // XỬ LÝ CHỈNH SỬA & XÓA (CHỦ SỞ HỮU)
  // ============================================================

  attachOwnerActions(commentNode, comment) {
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
        this.handleEdit(commentNode, comment);
      });
    }

    if (btnDelete) {
      btnDelete.addEventListener("click", (e) => {
        e.preventDefault();
        this.handleDelete(commentNode, comment);
      });
    }
  }

  handleEdit(commentNode, comment) {
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
        this.reRenderComment(commentNode, comment);
      });

    commentBubble
      .querySelector(".btn-save-edit")
      .addEventListener("click", async () => {
        await this.saveEdit(
          commentNode,
          comment,
          editTextArea,
          originalContent
        );
      });
  }

  async saveEdit(commentNode, comment, editTextArea, originalContent) {
    const newContent = editTextArea.value.trim();

    if (!newContent || newContent === originalContent) {
      this.reRenderComment(commentNode, comment);
      return;
    }

    try {
      const response = await fetch("/api/Comment/update", {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          RequestVerificationToken: this.config.antiForgeryToken,
        },
        body: JSON.stringify({
          commentId: comment.commentId,
          content: newContent,
        }),
      });

      if (!response.ok) {
        const result = await response.json().catch(() => ({}));
        throw new Error(result.message || "Lỗi server");
      }

      comment.content = newContent;
      this.reRenderComment(commentNode, comment);
      notify("Đã cập nhật bình luận.", "Thành công");
    } catch (error) {
      const errorElement = commentNode.querySelector(".edit-error");
      errorElement.textContent = error.message || "Lỗi. Vui lòng thử lại.";
      notify(error.message || "Lỗi. Vui lòng thử lại.", "Lỗi");
    }
  }

  async handleDelete(commentNode, comment) {
    const doDelete = async () => {
      try {
        const response = await fetch("/api/Comment/delete", {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            RequestVerificationToken: this.config.antiForgeryToken,
          },
          body: JSON.stringify({ commentId: comment.commentId }),
        });

        if (!response.ok) {
          let reason = "Không thể xóa bình luận.";
          try {
            const r = await response.json();
            reason = r.message || reason;
          } catch (e) {}
          throw new Error(reason);
        }

        commentNode.style.transition = "opacity 0.5s ease";
        commentNode.style.opacity = "0";
        setTimeout(() => commentNode.remove(), 500);
        notify("Đã xóa bình luận.", "Hoàn tất");
      } catch (err) {
        const msg = err.message || "Không thể xóa bình luận. Vui lòng thử lại.";
        notify(msg, "Lỗi");
      }
    };

    // Confirm luôn qua modal
    confirmAction(
      "Bạn có chắc chắn muốn xóa bình luận này?",
      async () => await doDelete(),
      {
        /* có thể truyền thêm options nếu modalHelper hỗ trợ */
      }
    );
  }

  // ============================================================
  // XỬ LÝ TRẢ LỜI
  // ============================================================

  attachReplyHandler(commentNode, comment) {
    const replyButton = commentNode.querySelector(".btn-reply");
    if (!replyButton) return;

    const dynamicFormContainer = commentNode.querySelector(
      ".dynamic-form-container"
    );

    replyButton.addEventListener("click", (e) => {
      e.preventDefault();

      // Kiểm tra đăng nhập trước khi reply
      if (!this.config.isAuthenticated) {
        notify("Bạn cần đăng nhập để trả lời bình luận.", "Cần đăng nhập");
        return;
      }

      this.toggleReplyForm(dynamicFormContainer, comment);
    });
  }

  toggleReplyForm(container, comment) {
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
        await this.submitReply(replyForm, comment);
      });
  }

  async submitReply(replyForm, comment) {
    const content = replyForm.querySelector(".reply-content").value.trim();
    if (!content) {
      const errorElement = replyForm.querySelector(".reply-error");
      errorElement.textContent = "Vui lòng nhập nội dung phản hồi.";
      notify("Vui lòng nhập nội dung phản hồi.", "Thiếu nội dung");
      return;
    }

    try {
      const result = await this.apiCreateComment(content, comment.commentId);

      if (result.success) {
        const commentNode = replyForm.closest(".comment-item");
        const replyListContainer = commentNode.querySelector(
          ".reply-list-container"
        );
        this.renderComment(result.data, replyListContainer, true);
        replyForm.remove();
        notify("Đã gửi phản hồi.", "Thành công");
      } else {
        throw new Error(result.message);
      }
    } catch (error) {
      const errorElement = replyForm.querySelector(".reply-error");
      errorElement.textContent = error.message || "Lỗi. Thử lại.";
      notify(error.message || "Lỗi. Thử lại.", "Lỗi");
    }
  }

  // ============================================================
  // RENDER REPLIES VÀ NÚT TOGGLE
  // ============================================================

  renderReplies(commentNode, comment) {
    const replyListContainer = commentNode.querySelector(
      ".reply-list-container"
    );
    const hasReplies = comment.replies?.length > 0;

    if (!hasReplies) return;

    comment.replies.forEach((reply) => {
      this.renderComment(reply, replyListContainer);
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

  // ============================================================
  // APPEND TO DOM
  // ============================================================

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

  // ============================================================
  // API CALLS
  // ============================================================

  async apiCreateComment(content, parentCommentId = null) {
    const response = await fetch("/api/Comment/create", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        RequestVerificationToken: this.config.antiForgeryToken,
      },
      body: JSON.stringify({
        plantId: this.config.plantId,
        content: content,
        ...(parentCommentId && { parentCommentId }),
      }),
    });

    const result = await response.json().catch(() => ({}));

    // 401 Unauthorized
    if (response.status === 401) {
      throw new Error("Bạn cần đăng nhập để thực hiện hành động này.");
    }

    if (!response.ok) {
      throw new Error(result.message || "Đã xảy ra lỗi từ máy chủ.");
    }

    return result;
  }

  async apiReactComment(commentId, reactionType) {
    const response = await fetch("/api/Comment/react", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        RequestVerificationToken: this.config.antiForgeryToken,
      },
      body: JSON.stringify({ commentId, reactionType }),
    });

    const result = await response.json().catch(() => ({}));

    if (response.status === 401) {
      throw new Error("Bạn cần đăng nhập để thích bình luận.");
    }

    if (!response.ok) {
      throw new Error(result.message || "Không thể thực hiện hành động này.");
    }

    return result;
  }

  // ============================================================
  // HELPER METHODS
  // ============================================================

  reRenderComment(commentNode, comment) {
    const tempDiv = document.createElement("div");
    this.renderComment(comment, tempDiv);
    commentNode.replaceWith(tempDiv.firstChild);
  }

  timeAgo(date) {
    const now = new Date();
    const diffSeconds = Math.floor((now - date) / 1000);

    if (diffSeconds < 60) return `${diffSeconds} giây trước`;

    const diffMinutes = Math.floor(diffSeconds / 60);
    if (diffMinutes < 60) return `${diffMinutes} phút trước`;

    const diffHours = Math.floor(diffMinutes / 60);
    if (diffHours < 24) return `${diffHours} giờ trước`;

    const diffDays = Math.floor(diffHours / 24);
    return `${diffDays} ngày trước`;
  }

  showError(message, container) {
    container.innerHTML = `<p class="text-danger">${message}</p>`;
    notify(message, "Lỗi");
  }

  showFormError(message) {
    const { commentError } = this.elements;
    if (commentError) {
      commentError.textContent = message;
      commentError.classList.remove("d-none");
    }
  }
}

// ============================================================
// KHỞI CHẠY
// ============================================================

document.addEventListener("DOMContentLoaded", () => {
  new CommentManager();
});
