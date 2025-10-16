// wwwroot/js/user-admin-page/users/events-list.js
import { userApi } from "./api.js";
import { renderUserList, renderHistoryTable } from "./render.js";
import { notify } from "../../helpers/notify.js";
import { $ } from "../core/dom.js";

export function bindListEvents() {
  const tbody = $("#userTableBody");

  async function reload({ q = "", status = "", page = 1, pageSize = 10 } = {}) {
    try {
      if (!tbody) {
        console.error("[events-list] Missing #userTableBody");
        notify("Thiếu phần tử bảng (#userTableBody).", "Lỗi cấu hình");
        return;
      }
      const { items = [] } = await userApi.list(q, status, page, pageSize);
      renderUserList(tbody, items);
    } catch (e) {
      console.error("[events-list] reload fail:", e);
      notify("Không tải được danh sách!", "Lỗi");
    }
  }

  document.addEventListener("click", async (e) => {
    const btn = e.target.closest(".btn-history");
    if (!btn) return;

    const userId = btn.dataset.userId;
    const row = document.getElementById(`history-row-${userId}`);
    const panel = document.getElementById(`history-panel-${userId}`);
    const tbodyEl = document.getElementById(`history-tbody-${userId}`);
    const emptyEl = document.getElementById(`history-empty-${userId}`);
    const loadingEl = document.getElementById(`history-loading-${userId}`);

    const expanded = btn.getAttribute("aria-expanded") === "true";

    // Thu gọn
    if (expanded) {
      btn.setAttribute("aria-expanded", "false");
      panel.classList.remove("show");
      setTimeout(() => (row.style.display = "none"), 400);
      return;
    }

    // Mở ra
    btn.setAttribute("aria-expanded", "true");
    row.style.display = "";
    setTimeout(() => panel.classList.add("show"), 10);

    if (!tbodyEl.dataset.loaded) {
      loadingEl.hidden = false;
      emptyEl.hidden = true;
      try {
        const { items = [] } = await userApi.history(userId, 10);
        if (items.length === 0) emptyEl.hidden = false;
        else {
          renderHistoryTable(tbodyEl, items);
          tbodyEl.dataset.loaded = "1";
        }
      } catch (err) {
        console.error("[history] fail:", err);
        notify("Không tải được lịch sử!", "Lỗi");
        emptyEl.hidden = false;
        emptyEl.textContent = "Không tải được lịch sử.";
      } finally {
        loadingEl.hidden = true;
      }
    }
  });

  return { reload };
}
