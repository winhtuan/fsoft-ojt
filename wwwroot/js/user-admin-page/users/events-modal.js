// users/events-modal.js
import { $ } from "../core/dom.js";
import { escapeHtml } from "../core/format.js";
import { notify } from "../../helpers/notify.js";
import { userApi } from "./api.js";

let currentModalMode = null; // 'create' | 'edit'
let currentUserId = null;
let modalInstance = null;

/**
 * Khởi tạo modal Bootstrap
 */
function initModal() {
  const modalEl = $("#userFormModal");
  if (!modalEl) return;

  // Khởi tạo Bootstrap Modal nếu chưa có
  if (!modalInstance && window.bootstrap) {
    modalInstance = new bootstrap.Modal(modalEl);
  }
}

/**
 * Mở modal ở chế độ tạo mới
 */
export function openCreateModal() {
  currentModalMode = "create";
  currentUserId = null;
  renderModalContent({
    title: "Thêm người dùng mới",
    submitText: "Tạo người dùng",
    submitClass: "btn-primary",
  });
  showModal();
}

/**
 * Mở modal ở chế độ chỉnh sửa
 * @param {number} userId - ID người dùng cần sửa
 * @param {object} userData - Dữ liệu người dùng hiện tại
 */
export async function openEditModal(userId, userData = null) {
  currentModalMode = "edit";
  currentUserId = userId;

  // Nếu không có userData, fetch từ API
  if (!userData) {
    try {
      userData = await userApi.getById(userId);
    } catch (err) {
      notify("Không thể tải thông tin người dùng!", "Lỗi");
      console.error("Lỗi tải user data:", err);
      return;
    }
  }

  renderModalContent({
    title: "Chỉnh sửa người dùng",
    submitText: "Cập nhật",
    submitClass: "btn-success",
    data: userData,
  });
  showModal();
}

/**
 * Render nội dung modal
 */
function renderModalContent(config) {
  const modalContent = $(".modal-content", $("#userFormModal"));
  if (!modalContent) return;

  const isEdit = currentModalMode === "edit";
  const data = config.data || {};

  const html = `
    <div class="modal-header">
      <h5 class="modal-title">
        <i class="fas ${isEdit ? "fa-pen" : "fa-plus"} me-2"></i>
        ${escapeHtml(config.title)}
      </h5>
      <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
    </div>
    <div class="modal-body">
      <form id="userFormModalForm" novalidate>
        ${
          isEdit
            ? `<input type="hidden" name="userId" value="${data.userId || ""}">`
            : ""
        }
        
        <div class="row g-3">
          <!-- Username -->
          <div class="col-md-6">
            <label for="modalUsername" class="form-label">
              Tên đăng nhập <span class="text-danger">*</span>
            </label>
            <input 
              type="text" 
              class="form-control" 
              id="modalUsername" 
              name="username"
              value="${escapeHtml(data.username || "")}"
              ${isEdit ? "readonly" : "required"}
              placeholder="username123">
            ${
              isEdit
                ? '<small class="text-muted">Không thể thay đổi username</small>'
                : '<div class="invalid-feedback">Vui lòng nhập tên đăng nhập</div>'
            }
          </div>

          <!-- Email -->
          <div class="col-md-6">
            <label for="modalEmail" class="form-label">
              Email <span class="text-danger">*</span>
            </label>
            <input 
              type="email" 
              class="form-control" 
              id="modalEmail" 
              name="email"
              value="${escapeHtml(data.email || "")}"
              ${isEdit ? "readonly" : "required"}
              placeholder="user@example.com">
            ${
              isEdit
                ? '<small class="text-muted">Không thể thay đổi email</small>'
                : '<div class="invalid-feedback">Vui lòng nhập email hợp lệ</div>'
            }
          </div>

          <!-- Họ và tên -->
          <div class="col-12">
            <label for="modalName" class="form-label">
              Họ và tên <span class="text-danger">*</span>
            </label>
            <input 
              type="text" 
              class="form-control" 
              id="modalName" 
              name="name"
              value="${escapeHtml(data.lastName || "")}"
              required
              placeholder="Nguyễn Văn A">
            <div class="invalid-feedback">Vui lòng nhập họ tên</div>
          </div>

          <!-- Giới tính -->
          <div class="col-md-6">
            <label class="form-label">Giới tính</label>
            <div>
              <div class="form-check form-check-inline">
                <input 
                  class="form-check-input" 
                  type="radio" 
                  name="gender" 
                  id="modalGenderM" 
                  value="M"
                  ${!data.gender || data.gender === "M" ? "checked" : ""}>
                <label class="form-check-label" for="modalGenderM">Nam</label>
              </div>
              <div class="form-check form-check-inline">
                <input 
                  class="form-check-input" 
                  type="radio" 
                  name="gender" 
                  id="modalGenderF" 
                  value="F"
                  ${data.gender === "F" ? "checked" : ""}>
                <label class="form-check-label" for="modalGenderF">Nữ</label>
              </div>
            </div>
          </div>

          <!-- Ngày sinh -->
          <div class="col-md-6">
            <label for="modalDob" class="form-label">
              Ngày sinh <span class="text-danger">*</span>
            </label>
            <input 
              type="date" 
              class="form-control" 
              id="modalDob" 
              name="dateOfBirth"
              value="${formatDateForInput(data.dateOfBirth)}"
              required
              max="${new Date().toISOString().split("T")[0]}">
            <div class="invalid-feedback">Vui lòng chọn ngày sinh</div>
          </div>

          <!-- Avatar URL -->
          <div class="col-12">
            <label for="modalAvatarUrl" class="form-label">URL Avatar</label>
            <input 
              type="url" 
              class="form-control" 
              id="modalAvatarUrl" 
              name="avatarUrl"
              value="${escapeHtml(data.avatarUrl || "")}"
              placeholder="https://example.com/avatar.jpg">
            <div class="form-text">Để trống sẽ dùng avatar mặc định</div>
          </div>

          <!-- Password (chỉ hiện khi tạo mới) -->
          ${
            !isEdit
              ? `
          <div class="col-12">
            <label for="modalPassword" class="form-label">
              Mật khẩu <span class="text-danger">*</span>
            </label>
            <input 
              type="password" 
              class="form-control" 
              id="modalPassword" 
              name="password"
              required
              minlength="6"
              placeholder="Tối thiểu 6 ký tự">
            <div class="invalid-feedback">Mật khẩu phải có ít nhất 6 ký tự</div>
          </div>
          `
              : ""
          }

          ${
            isEdit
              ? `
          <!-- Trạng thái (chỉ hiện khi sửa) -->
          <div class="col-12">
            <label for="modalStatus" class="form-label">Trạng thái</label>
            <select class="form-select" id="modalStatus" name="status">
              <option value="1" ${
                data.status === 1 ? "selected" : ""
              }>Hoạt động</option>
              <option value="2" ${
                data.status === 2 ? "selected" : ""
              }>Tạm khóa</option>
              <option value="3" ${
                data.status === 3 ? "selected" : ""
              }>Đã xóa</option>
            </select>
          </div>
          `
              : ""
          }
        </div>
      </form>
    </div>
    <div class="modal-footer">
      <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
        <i class="fas fa-times me-1"></i> Hủy
      </button>
      <button type="submit" form="userFormModalForm" class="btn ${
        config.submitClass
      }" id="modalSubmitBtn">
        <i class="fas fa-check me-1"></i> ${escapeHtml(config.submitText)}
      </button>
    </div>
  `;

  modalContent.innerHTML = html;

  // Gắn sự kiện submit
  const form = $("#userFormModalForm");
  if (form) {
    form.addEventListener("submit", handleSubmit);
  }
}

/**
 * Hiển thị modal
 */
function showModal() {
  initModal();
  if (modalInstance) {
    modalInstance.show();
  }
}

/**
 * Ẩn modal
 */
function hideModal() {
  if (modalInstance) {
    modalInstance.hide();
  }
}

/**
 * Xử lý submit form
 */
async function handleSubmit(e) {
  e.preventDefault();

  const form = e.target;

  // Validate HTML5
  if (!form.checkValidity()) {
    form.classList.add("was-validated");
    return;
  }

  const fd = new FormData(form);
  const dto = {
    username: fd.get("username")?.trim(),
    email: fd.get("email")?.trim(),
    name: fd.get("name")?.trim(),
    gender: (fd.get("gender") || "M").toString().toUpperCase()[0],
    dateOfBirth: fd.get("dateOfBirth"),
    avatarUrl: fd.get("avatarUrl")?.trim() || null,
  };

  // Disable submit button
  const submitBtn = $("#modalSubmitBtn");
  const originalText = submitBtn?.innerHTML;
  if (submitBtn) {
    submitBtn.disabled = true;
    submitBtn.innerHTML =
      '<i class="fas fa-spinner fa-spin me-1"></i> Đang xử lý...';
  }

  try {
    if (currentModalMode === "create") {
      // Tạo mới
      dto.password = fd.get("password");

      if (!dto.username || !dto.email || !dto.password) {
        notify("Vui lòng điền đầy đủ thông tin bắt buộc!", "Thiếu dữ liệu");
        return;
      }

      await userApi.create(dto);
      notify("Tạo người dùng thành công!", "Thành công");
    } else if (currentModalMode === "edit") {
      // Cập nhật
      dto.userId = currentUserId;
      dto.status = parseInt(fd.get("status") || "1", 10);

      await userApi.update(dto);
      notify("Cập nhật người dùng thành công!", "Thành công");
    }

    hideModal();

    // Trigger custom event để reload danh sách
    window.dispatchEvent(new CustomEvent("userListChanged"));
  } catch (err) {
    console.error("Lỗi khi submit form:", err);
    notify(
      currentModalMode === "create"
        ? "Không thể tạo người dùng. Vui lòng thử lại!"
        : "Không thể cập nhật người dùng. Vui lòng thử lại!",
      "Lỗi"
    );
  } finally {
    // Re-enable submit button
    if (submitBtn && originalText) {
      submitBtn.disabled = false;
      submitBtn.innerHTML = originalText;
    }
  }
}

/**
 * Format date cho input type="date"
 */
function formatDateForInput(isoDate) {
  if (!isoDate) return "";
  try {
    const d = new Date(isoDate);
    if (isNaN(d.getTime())) return "";
    return d.toISOString().split("T")[0];
  } catch {
    return "";
  }
}

/**
 * Bind sự kiện cho nút "Thêm người dùng"
 */
export function bindCreateButton() {
  const btnCreate = $("#btnCreateUser");
  if (btnCreate) {
    btnCreate.addEventListener("click", () => {
      openCreateModal();
    });
  }
}

/**
 * Export hàm để dùng từ events-list.js khi click nút Edit
 */
export { openEditModal as openUserEditModal };
