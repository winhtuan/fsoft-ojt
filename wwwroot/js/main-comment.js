// ============================================================
// ENTRY POINT - Khởi chạy ứng dụng
// ============================================================

import { CommentManager } from "./comment-system/comment-manager.js";

document.addEventListener("DOMContentLoaded", () => {
  new CommentManager();
});
