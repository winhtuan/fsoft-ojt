import { notify } from "../../helpers/notify.js";
import { userApi } from "./api.js";

/**
 * Gắn sự kiện cho form tạo mới người dùng.
 * @param {Function} reload - Hàm reload danh sách người dùng (truyền từ main-user-admin.js)
 */
export function bindCreateForm(reload) {
  const form = document.getElementById("formCreateUser");
  if (!form) {
    console.warn("[bindCreateForm] Không tìm thấy form #formCreateUser");
    return;
  }

  form.addEventListener("submit", async (e) => {
    e.preventDefault();

    // Gom dữ liệu từ form
    const fd = new FormData(form);
    const dto = {
      username: fd.get("username")?.trim(),
      email: fd.get("email")?.trim(),
      name: fd.get("name")?.trim(),
      gender: (fd.get("gender") || "M").toString().toUpperCase()[0],
      dateOfBirth: fd.get("dateOfBirth"),
      avatarUrl: fd.get("avatarUrl")?.trim(),
      password: fd.get("password"),
    };

    // Kiểm tra sơ bộ
    if (!dto.username || !dto.email || !dto.password) {
      notify("Vui lòng điền đầy đủ thông tin bắt buộc!", "Thiếu dữ liệu");
      return;
    }

    // Gọi API tạo user
    try {
      await userApi.create(dto);
      notify("Tạo người dùng thành công!", "Thành công");
      form.reset();
      await reload();
    } catch (err) {
      console.error("Lỗi khi tạo người dùng:", err);
      notify("Không thể tạo người dùng. Vui lòng thử lại sau!", "Lỗi");
    }
  });
}
