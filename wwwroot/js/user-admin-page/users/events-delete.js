// users/events-delete.js
import { userApi } from "./api.js";
import { confirmAction, notify } from "../../helpers/notify.js";

export function bindDeleteRestore(reload) {
  document.addEventListener("click", async (e) => {
    const del = e.target.closest(".btn-delete");
    const restore = e.target.closest(".btn-restore");
    if (!del && !restore) return;

    const id = (del || restore).dataset.userId;
    const isDelete = !!del;

    // confirm bằng modalHelper
    let confirmed = false;
    await confirmAction(
      isDelete ? "Xoá người dùng này?" : "Khôi phục người dùng này?",
      () => {
        confirmed = true;
      }
    );
    if (!confirmed) return;

    try {
      if (isDelete) await userApi.delete(id);
      else await userApi.restore(id);
      notify(isDelete ? "Đã xoá!" : "Đã khôi phục!", "Thành công");
      await reload();
    } catch {
      notify("Thao tác thất bại!", "Lỗi");
    }
  });
}
