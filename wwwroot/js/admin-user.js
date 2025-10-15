class AdminUsersManager {
  constructor() {
    this.users = [];
    this.currentPage = 1;
    this.itemsPerPage = 12;
    this.selectedUsers = new Set();
    this.init();
  }

  init() {
    this.bindEvents();
    this.loadUsers();
    this.setupModals();
  }

  bindEvents() {
    // Search and filters
    document
      .getElementById("globalSearch")
      .addEventListener("input", debounce(this.searchUsers.bind(this), 300));
    document.querySelectorAll(".filter-btn").forEach((btn) => {
      btn.addEventListener("click", () =>
        this.applyQuickFilter(btn.dataset.filter)
      );
    });

    // Bulk actions
    document
      .getElementById("btnCreateUser")
      .addEventListener("click", () => this.showCreateModal());

    // Pagination
    document.addEventListener("click", (e) => {
      if (e.target.classList.contains("page-link")) {
        this.currentPage = parseInt(e.target.dataset.page);
        this.renderUsers();
      }
    });
  }

  async loadUsers(filters = {}) {
    try {
      this.showLoading();
      // API call
      const response = await fetch("/admin/users", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          ...filters,
          page: this.currentPage,
          pageSize: this.itemsPerPage,
        }),
      });

      const data = await response.json();
      this.users = data.users;
      this.updateStats(data.stats);
      this.renderUsers();
      this.renderPagination(data.totalPages, data.totalItems);
      this.hideLoading();
    } catch (error) {
      console.error("Error loading users:", error);
      this.hideLoading();
    }
  }

  renderUsers() {
    const grid = document.getElementById("usersGrid");
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    const paginatedUsers = this.users.slice(start, end);

    grid.innerHTML = paginatedUsers
      .map((user) => this.createUserCard(user))
      .join("");

    // Update bulk actions visibility
    this.updateBulkActions();
  }

  createUserCard(user) {
    const statusClass = `status-${user.status.toLowerCase()}`;
    const statusBadgeClass = `status-badge status-${user.status.toLowerCase()}`;

    return `
            <div class="user-card ${statusClass} ${
      this.selectedUsers.has(user.userId) ? "selected" : ""
    }" 
                 data-user-id="${user.userId}">
                <input type="checkbox" class="user-card-checkbox" value="${
                  user.userId
                }" 
                       ${this.selectedUsers.has(user.userId) ? "checked" : ""}>
                
                <div class="user-actions">
                    <button class="action-btn btn-view" title="Xem chi tiết">
                        <i class="fas fa-eye"></i>
                    </button>
                    <button class="action-btn btn-edit" title="Chỉnh sửa">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn btn-delete" title="Xóa">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
                
                <div class="user-header">
                    <img src="${
                      user.avatarUrl || "/images/default-avatar.png"
                    }" 
                         alt="${user.username}" class="user-avatar">
                    <div class="user-info">
                        <h3>${user.username} (${user.email})</h3>
                        <span class="${statusBadgeClass}">${user.status}</span>
                    </div>
                </div>
                
                <div class="user-meta">
                    <div class="user-meta-item">
                        <i class="fas fa-user"></i>
                        ${user.lastName || "N/A"}
                    </div>
                    <div class="user-meta-item">
                        <i class="fas fa-calendar"></i>
                        ${this.formatDate(user.lastLoginAt) || "Chưa đăng nhập"}
                    </div>
                    ${
                      user.role
                        ? `<div class="user-meta-item">
                        <i class="fas fa-user-tag"></i>
                        ${user.role}
                    </div>`
                        : ""
                    }
                </div>
                
                <div class="activity-stats">
                    <div class="stat-item">
                        <div class="stat-number">${user.commentCount}</div>
                        <div class="stat-label">Bình luận</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-number">${user.reactionCount}</div>
                        <div class="stat-label">Thích</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-number">${user.searchCount}</div>
                        <div class="stat-label">Tìm kiếm</div>
                    </div>
                </div>
            </div>
        `;
  }

  // Bulk selection
  updateBulkActions() {
    const count = this.selectedUsers.size;
    const bulkActions = document.getElementById("bulkActions");
    const selectedCount = document.getElementById("selectedCount");

    if (count > 0) {
      selectedCount.textContent = count;
      bulkActions.style.display = "flex";
      document.querySelectorAll(".user-card").forEach((card) => {
        card.classList.toggle(
          "selected",
          this.selectedUsers.has(parseInt(card.dataset.userId))
        );
      });
    } else {
      bulkActions.style.display = "none";
    }
  }

  // Modal management
  showCreateModal() {
    // Render create form
    const modal = new bootstrap.Modal(document.getElementById("userFormModal"));
    modal.show();
  }

  // Utility methods
  formatDate(date) {
    return date ? new Date(date).toLocaleDateString("vi-VN") : null;
  }

  showLoading() {
    if (!document.querySelector(".loading-overlay")) {
      document.body.insertAdjacentHTML(
        "beforeend",
        `
                <div class="loading-overlay">
                    <div class="spinner"></div>
                </div>
            `
      );
    }
  }

  hideLoading() {
    document.querySelector(".loading-overlay")?.remove();
  }

  debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
      const later = () => {
        clearTimeout(timeout);
        func(...args);
      };
      clearTimeout(timeout);
      timeout = setTimeout(later, wait);
    };
  }
}

// Initialize when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
  window.adminUsersManager = new AdminUsersManager();
});
