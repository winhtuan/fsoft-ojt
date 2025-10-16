// users/events-update.js
import { userApi } from "./api.js";
import { notify } from "../../helpers/notify.js";

export function bindUpdateFlow(reload) {
  document.addEventListener("click", async (e) => {
    const btn = e.target.closest(".btn-update");
    if (!btn) return;

    const id = Number(btn.dataset.userId);

    // TODO: mở modal chỉnh sửa, lấy dữ liệu form, sau đó submit:
    try {
      // ví dụ tối giản: chỉ gửi userId (bạn thay dto thực tế từ form)
      await userApi.update({ userId: id });
      notify("Cập nhật thành công!", "Thành công");
      await reload();
    } catch {
      notify("Cập nhật thất bại!", "Lỗi");
    }
  });
}
