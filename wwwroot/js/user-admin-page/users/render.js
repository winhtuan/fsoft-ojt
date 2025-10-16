// users/render.js
import { escapeHtml, formatDateVN } from "../core/format.js";
import { clearChildren } from "../core/dom.js";

function statusBadge(st) {
  // 1: Active, 2: Suspended, 3: Deleted
  switch (st) {
    case 1:
      return '<span class="badge bg-success">Hoạt động</span>';
    case 2:
      return '<span class="badge bg-warning text-dark">Tạm khóa</span>';
    case 3:
      return '<span class="badge bg-secondary">Đã xóa</span>';
    default:
      return '<span class="badge bg-light text-dark">—</span>';
  }
}

function statSummary(u) {
  const c = Number(u.commentCount || 0);
  const r = Number(u.reactionCount || 0);
  const s = Number(u.searchCount || 0);

  return `
    <div class="d-flex flex-column align-items-end gap-1 text-muted small stat-summary">
      <div class="badge stat-badge comment px-2 py-1 w-100 text-start">
        Bình luận: <strong>${c}</strong>
      </div>
      <div class="badge stat-badge reaction px-2 py-1 w-100 text-start">
        Thích: <strong>${r}</strong>
      </div>
      <div class="badge stat-badge search px-2 py-1 w-100 text-start">
        Tìm kiếm: <strong>${s}</strong>
      </div>
    </div>
  `;
}

/**
 * Render danh sách người dùng dạng bảng + hàng lịch sử ẩn kèm theo
 * Cần trùng số cột với thead của bạn (7 cột)
 */
export function renderUserList(container, users = []) {
  if (!container) return;
  clearChildren(container);

  for (const u of users) {
    const id = Number(u.userId);
    const created = u.createdAt ? formatDateVN(u.createdAt) : "-";
    const lastLogin = u.lastLoginAt ? formatDateVN(u.lastLoginAt) : "-";

    // --- HÀNG CHÍNH ---
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td>
        <div class="d-flex align-items-center gap-2">
          <div class="rounded overflow-hidden" style="width:36px;height:36px">
            <img src="${escapeHtml(
              u.avatarUrl || "/images/avatar-placeholder.png"
            )}"
                 onerror="this.onerror=null;this.src='/images/avatar-placeholder.png';"
                 alt="${escapeHtml(u.lastName || u.username || "")}"
                 style="width:36px;height:36px;object-fit:cover">
          </div>
          <div>
            <div class="text-muted small">@${escapeHtml(u.username || "")}</div>
          </div>
        </div>
      </td>
      <td>${escapeHtml(u.email || "")}</td>
      <td>${statusBadge(u.status)}</td>
      <td>${created}</td>
      <td>${lastLogin}</td>
      <td class="text-end">${statSummary(u)}</td>
      <td class="text-center">
        <div class="actions">
          <button class="icon-btn btn-history"
                  title="Lịch sử"
                  data-user-id="${id}" aria-expanded="false">
            <i class="fas fa-chevron-down"></i>
          </button>
          <button class="icon-btn btn-edit btn-update" title="Sửa" data-user-id="${id}">
            <i class="fas fa-pen"></i>
          </button>
          <button class="icon-btn btn-danger-soft btn-delete" title="Xoá" data-user-id="${id}">
            <i class="fas fa-trash"></i>
          </button>
        </div>
      </td>
    `;
    container.appendChild(tr);

    // --- HÀNG LỊCH SỬ (ẨN) ---
    const detail = document.createElement("tr");
    detail.id = `history-row-${id}`;
    detail.innerHTML = `
      <td colspan="7" class="bg-light">
        <div class="history-panel" id="history-panel-${id}"> 
          <div class="d-flex align-items-center justify-content-between mb-2">
            <div class="fw-semibold"><i class="fas fa-clock me-2"></i>Lịch sử hoạt động</div>
          </div>

          <div id="history-loading-${id}" class="py-2" hidden>Đang tải...</div>
          <div id="history-empty-${id}" class="text-muted py-2" hidden>Chưa có hoạt động.</div>

          <div class="history-scroll">
            <table class="table table-sm align-middle history-table mb-0">
              <thead>
                <tr>
                  <th style="width:52px">#</th>
                  <th style="width:120px">Loại</th>
                  <th>Mô tả</th>
                  <th style="width:180px">Thời gian</th>
                </tr>
              </thead>
              <tbody id="history-tbody-${id}"></tbody>
            </table>
          </div>
        </div>
      </td>
    `;
    detail.style.display = "none";
    container.appendChild(detail);
  }
}

// Đổ dữ liệu vào tbody của bảng lịch sử
export function renderHistoryTable(tbodyEl, items = []) {
  if (!tbodyEl) return;
  clearChildren(tbodyEl);

  items.forEach((a, idx) => {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td class="text-muted">${idx + 1}</td>
      <td><span class="badge bg-light text-dark">${escapeHtml(
        a.type || "—"
      )}</span></td>
      <td>${escapeHtml(a.description || "")}</td>
      <td class="text-nowrap text-muted">${
        a.createdAt ? formatDateVN(a.createdAt) : "-"
      }</td>
    `;
    tbodyEl.appendChild(tr);
  });
}
