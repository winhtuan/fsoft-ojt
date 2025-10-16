// main-user-admin.js
import { bindListEvents } from "./users/events-list.js";
import { bindDeleteRestore } from "./users/events-delete.js";
import { bindCreateButton } from "./users/events-modal.js";
import { notify } from "../helpers/notify.js";

document.addEventListener("DOMContentLoaded", async () => {
  try {
    // Gắn các sự kiện trang người dùng
    const { reload } = bindListEvents();
    bindDeleteRestore(reload);
    bindCreateButton();

    // Lắng nghe sự kiện reload từ modal
    window.addEventListener("userListChanged", reload);

    // Tải danh sách ban đầu
    await reload();
  } catch (err) {
    console.error("[AdminUserInit] Lỗi khởi tạo:", err);
    notify("Không thể tải danh sách người dùng. Vui lòng thử lại sau!", "Lỗi");
  }
});
