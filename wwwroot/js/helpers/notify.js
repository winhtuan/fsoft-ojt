// ============================================================
// NOTIFY HELPERS - Luôn hiển thị bằng popup (modalHelper)
// wwwroot/js/comment-system/helpers/notify.js
// ============================================================

export function notify(message, title = "Thông báo") {
  if (
    window.modalHelper &&
    typeof window.modalHelper.showAlert === "function"
  ) {
    return window.modalHelper.showAlert(message, title);
  }
  console.error("[ModalHelper Missing] ", title, message);
}

export function confirmAction(message, onOk, options) {
  if (
    window.modalHelper &&
    typeof window.modalHelper.showConfirm === "function"
  ) {
    return window.modalHelper.showConfirm(message, onOk, options);
  }
  console.error("[ModalHelper Missing] Xác nhận:", message);
}
