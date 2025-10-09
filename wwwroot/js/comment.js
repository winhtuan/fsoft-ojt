// Đợi cho toàn bộ nội dung trang được tải xong trước khi chạy script
document.addEventListener("DOMContentLoaded", function () {
  // --- Lấy các phần tử DOM cần thiết ---
  const commentSection = document.querySelector("section[data-plant-id]");
  if (!commentSection) {
    console.error("Không tìm thấy khu vực bình luận.");
    return;
  }

  const plantId = commentSection.dataset.plantId;
  const commentList = document.getElementById("commentList");
  const commentTemplate = document.getElementById("commentTemplate");
  const btnSubmitComment = document.getElementById("btnSubmitComment");
  const commentContent = document.getElementById("commentContent");
  const commentError = document.getElementById("commentError");

  // Lấy Anti-Forgery Token (Rất quan trọng để gửi request POST)
  const antiForgeryToken = document.querySelector(
    'input[name="__RequestVerificationToken"]'
  ).value;

  /**
   * Hàm render một bình luận dựa trên dữ liệu và template
   * @param {object} comment - Dữ liệu của một bình luận (từ API)
   * @param {boolean} prepend - Nếu true, thêm bình luận lên đầu danh sách (cho bình luận mới)
   */
  function renderComment(comment, prepend = false) {
    if (!commentTemplate) return;

    // Sao chép nội dung từ template
    const commentNode = commentTemplate.content.cloneNode(true);

    // Điền dữ liệu vào các phần tử tương ứng
    commentNode.querySelector(".comment-author").textContent =
      comment.authorName || "Người dùng ẩn danh";
    commentNode.querySelector(".comment-text").textContent = comment.content;
    commentNode.querySelector(".react-count").textContent =
      comment.reactionCount || 0;

    // Định dạng lại ngày tháng cho dễ đọc
    const commentDate = new Date(comment.createdAt);
    commentNode.querySelector(".comment-date").textContent =
      commentDate.toLocaleString("vi-VN");

    // Thêm bình luận vào danh sách
    if (prepend) {
      commentList.prepend(commentNode);
    } else {
      commentList.appendChild(commentNode);
    }
  }

  /**
   * Hàm tải tất cả bình luận của cây trồng từ API
   */
  async function loadComments() {
    try {
      const response = await fetch(`/api/comment/${plantId}`);

      if (!response.ok) {
        throw new Error("Không thể tải bình luận.");
      }

      const result = await response.json();

      if (result.success && result.data) {
        commentList.innerHTML = ""; // Xóa các bình luận cũ trước khi tải mới
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

  /**
   * Hàm xử lý việc gửi bình luận mới
   */
  async function handleSubmitComment() {
    const content = commentContent.value.trim();

    if (!content) {
      commentError.textContent = "Vui lòng nhập nội dung bình luận.";
      commentError.classList.remove("d-none");
      return;
    }

    // Ẩn thông báo lỗi nếu có
    commentError.classList.add("d-none");

    try {
      const response = await fetch("/api/comment/create", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          // Đính kèm Anti-Forgery Token vào header
          RequestVerificationToken: antiForgeryToken,
        },
        body: JSON.stringify({
          plantId: plantId,
          content: content,
        }),
      });

      const result = await response.json();

      if (response.ok && result.success) {
        // Thêm bình luận mới vào đầu danh sách
        renderComment(result.data, true);
        // Xóa nội dung trong textarea
        commentContent.value = "";
        // Nếu trước đó chưa có bình luận nào, xóa thông báo
        if (commentList.querySelector("p")) {
          commentList.innerHTML = "";
        }
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

  // --- Gắn các sự kiện ---

  // Gắn sự kiện click cho nút "Gửi bình luận"
  // Chỉ gắn sự kiện nếu người dùng đã đăng nhập (nút tồn tại)
  if (btnSubmitComment) {
    btnSubmitComment.addEventListener("click", handleSubmitComment);
  }

  // Tải danh sách bình luận khi trang được mở
  loadComments();
});
