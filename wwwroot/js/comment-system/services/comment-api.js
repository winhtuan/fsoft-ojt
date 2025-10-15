// ============================================================
// COMMENT API SERVICE - Xử lý tất cả API calls
// ============================================================

export class CommentAPI {
  constructor(config) {
    this.config = config;
  }

  async loadComments(plantId) {
    const response = await fetch(`/api/Comment/${plantId}`);
    if (!response.ok) throw new Error("Không thể tải bình luận.");
    return await response.json();
  }

  async createComment(content, parentCommentId = null) {
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

    if (response.status === 401) {
      throw new Error("Bạn cần đăng nhập để thực hiện hành động này.");
    }

    if (!response.ok) {
      throw new Error(result.message || "Đã xảy ra lỗi từ máy chủ.");
    }

    return result;
  }

  async updateComment(commentId, content) {
    const response = await fetch("/api/Comment/update", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        RequestVerificationToken: this.config.antiForgeryToken,
      },
      body: JSON.stringify({ commentId, content }),
    });

    if (!response.ok) {
      const result = await response.json().catch(() => ({}));
      throw new Error(result.message || "Lỗi server");
    }

    return await response.json();
  }

  async deleteComment(commentId) {
    const response = await fetch("/api/Comment/delete", {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        RequestVerificationToken: this.config.antiForgeryToken,
      },
      body: JSON.stringify({ commentId }),
    });

    if (!response.ok) {
      let reason = "Không thể xóa bình luận.";
      try {
        const r = await response.json();
        reason = r.message || reason;
      } catch (e) {}
      throw new Error(reason);
    }
  }

  async reactComment(commentId, reactionType) {
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
}
