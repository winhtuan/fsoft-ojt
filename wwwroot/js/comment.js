document.addEventListener("DOMContentLoaded", function () {
  const commentSection = document.querySelector("section[data-plant-id]");
  if (!commentSection) {
    console.error("Không tìm thấy khu vực bình luận.");
    return;
  }

  const currentUserId = window.currentUserId;
  const plantId = commentSection.dataset.plantId;
  const commentList = document.getElementById("commentList");
  const commentTemplate = document.getElementById("commentTemplate");
  const btnSubmitComment = document.getElementById("btnSubmitComment");
  const commentContent = document.getElementById("commentContent");
  const commentError = document.getElementById("commentError");
  const antiForgeryToken = document.querySelector(
    'input[name="__RequestVerificationToken"]'
  )?.value;

  // ==== Hàm renderComment với nút ẩn/hiện reply chỉ khi có reply, và like/unlike mọi cấp ====
  function renderComment(comment, parentNode = null, prepend = false) {
    if (!commentTemplate) return;

    const commentNode = commentTemplate.content
      .cloneNode(true)
      .querySelector(".comment-item");
    const isOwner = String(currentUserId) === String(comment.userId);

    // AVATAR: Luôn gán src và log, fallback ảnh mặc định nếu lỗi
    const avatarImg = commentNode.querySelector(".comment-avatar img");
    const avatarIcon = commentNode.querySelector(
      ".comment-avatar .fa-user-circle"
    );
    const defaultAvatar = "/img/default-avatar.png";
    const avatarSrc =
      (comment.avatarUrl && comment.avatarUrl.trim()) || defaultAvatar;

    if (avatarImg && avatarIcon) {
      avatarImg.src = avatarSrc;
      avatarImg.style.display = "block";
      avatarIcon.style.display = "none";
      avatarImg.onerror = function () {
        avatarImg.style.display = "none";
        avatarIcon.style.display = "block";
      };
    }

    commentNode.querySelector(".comment-author").textContent =
      comment.userName || "Người dùng ẩn danh";
    // Nếu muốn định dạng thời gian như Facebook:
    commentNode.querySelector(".comment-date").textContent = timeAgo(
      new Date(comment.createdAt)
    );
    commentNode.querySelector(".comment-text").textContent = comment.content;
    const reactCountSpan = commentNode.querySelector(".react-count");
    if (reactCountSpan) reactCountSpan.textContent = comment.reactCount || 0;

    // === Like/Unlike (áp dụng cho mọi cấp comment) ===
    const reactButton = commentNode.querySelector(".btn-react");
    // Nếu API có trả về trạng thái đã react bởi currentUser thì đánh dấu class "active"
    if (reactButton && comment.isReactedByCurrentUser) {
      reactButton.classList.add("active");
    }
    if (reactButton) {
      reactButton.addEventListener("click", async function (e) {
        e.preventDefault();
        const hasReacted = reactButton.classList.contains("active");
        try {
          const response = await fetch("/api/Comment/react", {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              RequestVerificationToken: antiForgeryToken,
            },
            body: JSON.stringify({
              commentId: comment.commentId,
              reactionType: !hasReacted, // Đảo trạng thái
            }),
          });

          const result = await response.json();
          if (response.ok && result.success) {
            if (result.data && typeof result.data.reactCount !== "undefined") {
              reactCountSpan.textContent = result.data.reactCount;
            } else {
              let old = parseInt(reactCountSpan.textContent) || 0;
              reactCountSpan.textContent = hasReacted ? old - 1 : old + 1;
            }
            // Toggle trạng thái highlight nút
            if (hasReacted) {
              reactButton.classList.remove("active");
            } else {
              reactButton.classList.add("active");
            }
          } else {
            console.warn("Không thể phản hồi:", result.message);
          }
        } catch (error) {
          console.error("Lỗi khi gửi phản hồi:", error);
        }
      });
    }

    const dynamicFormContainer = commentNode.querySelector(
      ".dynamic-form-container"
    );
    const commentActions = commentNode.querySelector(".comment-actions");

    const replyButton = commentNode.querySelector(".btn-reply");
    if (replyButton) {
      if (!isOwner) {
        replyButton.style.display = "inline-block";
        replyButton.addEventListener("click", function (event) {
          event.preventDefault();
          const existingForm =
            dynamicFormContainer.querySelector(".reply-form");
          if (existingForm) {
            existingForm.remove();
            return;
          }
          dynamicFormContainer.innerHTML = "";
          const replyForm = document.createElement("div");
          replyForm.className = "reply-form mt-2";
          replyForm.innerHTML = `
                        <textarea class="form-control mb-1 reply-content" rows="2" placeholder="Phản hồi tới ${comment.userName}..."></textarea>
                        <div class="d-flex align-items-center">
                            <button class="btn btn-success btn-sm btn-submit-reply" type="button">Gửi</button>
                            <button class="btn btn-secondary btn-sm ms-2 btn-cancel-reply" type="button">Hủy</button>
                            <small class="text-danger ms-2 reply-error d-none"></small>
                        </div>
                    `;
          dynamicFormContainer.appendChild(replyForm);
          replyForm.querySelector(".reply-content").focus();
          replyForm
            .querySelector(".btn-cancel-reply")
            .addEventListener("click", () => replyForm.remove());
          replyForm
            .querySelector(".btn-submit-reply")
            .addEventListener("click", async function () {
              const content = replyForm
                .querySelector(".reply-content")
                .value.trim();
              const replyError = replyForm.querySelector(".reply-error");
              if (!content) {
                replyError.textContent = "Vui lòng nhập nội dung phản hồi.";
                replyError.classList.remove("d-none");
                return;
              }
              replyError.classList.add("d-none");
              try {
                const response = await fetch("/api/Comment/create", {
                  method: "POST",
                  headers: {
                    "Content-Type": "application/json",
                    RequestVerificationToken: antiForgeryToken,
                  },
                  body: JSON.stringify({
                    plantId: plantId,
                    content: content,
                    parentCommentId: comment.commentId,
                  }),
                });
                const result = await response.json();
                if (response.ok && result.success) {
                  let replyListContainer = commentNode.querySelector(
                    ".reply-list-container"
                  );
                  let replyList =
                    replyListContainer.querySelector(".reply-list");
                  if (!replyList) {
                    replyList = document.createElement("div");
                    replyList.className = "reply-list ms-4 mt-2";
                    replyListContainer.appendChild(replyList);
                  }
                  const newReplyNode = renderComment(result.data, replyList);
                  newReplyNode.classList.add("new-comment");
                  setTimeout(
                    () => newReplyNode.classList.remove("new-comment"),
                    1000
                  );
                  replyForm.remove();
                  if (toggleRepliesBtn) {
                    replyList.style.display = "";
                    toggleRepliesBtn.textContent = "Ẩn phản hồi";
                    repliesVisible = true;
                  }
                } else {
                  replyError.textContent =
                    result.message || "Không thể gửi phản hồi.";
                  replyError.classList.remove("d-none");
                }
              } catch (error) {
                replyError.textContent = "Lỗi kết nối. Vui lòng thử lại.";
                replyError.classList.remove("d-none");
              }
            });
        });
      } else {
        replyButton.style.display = "none";
      }
    }

    // ---- Chỉ bình luận gốc có replies mới có nút hiện/ẩn ----
    const isRoot = comment.parentCommentId == null;
    const hasReplies = comment.replies && comment.replies.length > 0;
    let toggleRepliesBtn = null;
    let repliesVisible = false;
    if (isRoot && hasReplies) {
      toggleRepliesBtn = document.createElement("a");
      toggleRepliesBtn.href = "#";
      toggleRepliesBtn.className =
        "small text-decoration-none ms-2 toggle-replies-btn";
      toggleRepliesBtn.textContent = "Hiện phản hồi";
      repliesVisible = false;
      toggleRepliesBtn.addEventListener("click", function (e) {
        e.preventDefault();
        const replyListElem = commentNode.querySelector(".reply-list");
        if (!replyListElem) return;
        repliesVisible = !repliesVisible;
        if (repliesVisible) {
          replyListElem.style.display = "";
          toggleRepliesBtn.textContent = "Ẩn phản hồi";
          replyListElem.style.animation = "fadeIn 0.5s ease";
        } else {
          replyListElem.style.animation = "fadeOut 0.5s ease";
          setTimeout(() => (replyListElem.style.display = "none"), 500);
          toggleRepliesBtn.textContent = "Hiện phản hồi";
        }
      });

      if (commentActions) {
        commentActions.appendChild(toggleRepliesBtn);
      }
    }

    // ---- Render replies (mọi cấp đều like/unlike được) ----
    if (hasReplies) {
      let replyListContainer = commentNode.querySelector(
        ".reply-list-container"
      );
      let replyList = document.createElement("div");
      replyList.className = "reply-list ms-4 mt-2";
      if (isRoot) replyList.style.display = "none";
      comment.replies.forEach((reply) => {
        renderComment(reply, replyList);
      });
      replyListContainer.appendChild(replyList);
    }

    if (parentNode) {
      parentNode.appendChild(commentNode);
    } else {
      if (prepend) {
        commentList.prepend(commentNode);
        commentNode.classList.add("new-comment");
        setTimeout(() => commentNode.classList.remove("new-comment"), 1000);
      } else {
        commentList.appendChild(commentNode);
      }
    }
    return commentNode;
  }

  // Hàm timeAgo cho thời gian kiểu Facebook
  function timeAgo(date) {
    const now = new Date();
    const diff = Math.floor((now - date) / 1000);
    if (diff < 60) return diff + " giây";
    if (diff < 3600) return Math.floor(diff / 60) + " phút";
    if (diff < 86400) return Math.floor(diff / 3600) + " giờ";
    return date.toLocaleString("vi-VN");
  }

  async function loadComments() {
    try {
      const response = await fetch(`/api/Comment/${plantId}`);
      if (!response.ok) throw new Error("Không thể tải bình luận.");
      const result = await response.json();
      if (result.success && result.data) {
        commentList.innerHTML = "";
        if (result.data.length === 0) {
          commentList.innerHTML =
            '<p class="text-muted">Chưa có bình luận nào. Hãy là người đầu tiên bình luận!</p>';
        } else {
          result.data.forEach((comment) => renderComment(comment));
        }
      } else {
        commentList.innerHTML =
          '<p class="text-danger">Đã xảy ra lỗi khi tải bình luận.</p>';
      }
    } catch (error) {
      console.error("Lỗi khi tải bình luận:", error);
      commentList.innerHTML =
        '<p class="text-danger">Đã xảy ra lỗi kết nối. Vui lòng thử lại.</p>';
    }
  }

  async function handleSubmitComment() {
    const content = commentContent.value.trim();
    if (!content) {
      commentError.textContent = "Vui lòng nhập nội dung bình luận.";
      commentError.classList.remove("d-none");
      return;
    }
    commentError.classList.add("d-none");
    try {
      const response = await fetch("/api/Comment/create", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          RequestVerificationToken: antiForgeryToken,
        },
        body: JSON.stringify({ plantId: plantId, content: content }),
      });
      const result = await response.json();
      if (response.ok && result.success) {
        renderComment(result.data, null, true);
        commentContent.value = "";
        const emptyMsg = commentList.querySelector("p.text-muted");
        if (emptyMsg) emptyMsg.remove();
      } else {
        commentError.textContent =
          result.message || "Không thể gửi bình luận. Vui lòng thử lại.";
        commentError.classList.remove("d-none");
      }
    } catch (error) {
      console.error("Lỗi khi gửi bình luận:", error);
      commentError.textContent = "Đã xảy ra lỗi kết nối. Vui lòng thử lại.";
      commentError.classList.remove("d-none");
    }
  }

  if (btnSubmitComment) {
    btnSubmitComment.addEventListener("click", function (e) {
      e.preventDefault();
      handleSubmitComment();
    });
  }
  loadComments();
});
