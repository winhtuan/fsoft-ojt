export function initUIEnhancements(opts = {}) {
  const {
    toggleBtnId = "btnToggleAdvanced",
    advPanelId = "advancedFilters",
    searchInputId = "globalSearch",
    clearBtnId = "btnSearchClear",
    rotateClass = "rotated",
  } = opts;

  const onReady = () => {
    const btn = document.getElementById(toggleBtnId);
    const panel = document.getElementById(advPanelId);
    if (btn && panel) {
      setupAdvancedToggle(btn, panel, rotateClass);
    }

    // 3) Nút xoá ô tìm kiếm
    const input = document.getElementById(searchInputId);
    const clear = document.getElementById(clearBtnId);
    if (input && clear) {
      setupSearchClear(input, clear);
    }
  };

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", onReady, { once: true });
  } else {
    onReady();
  }
}

// ========== Helpers ==========

function setupAdvancedToggle(btn, panel, rotateClass) {
  // Khởi tạo trạng thái ban đầu
  const icon = btn.querySelector(".toggle-icon");
  const isHidden =
    getComputedStyle(panel).display === "none" || panel.hidden === true;
  applyPanelState({ panel, btn, icon, rotateClass, expanded: !isHidden });

  btn.addEventListener("click", () => {
    const expanded = btn.getAttribute("aria-expanded") === "true";
    applyPanelState({ panel, btn, icon, rotateClass, expanded: !expanded });
  });
}

function applyPanelState({ panel, btn, icon, rotateClass, expanded }) {
  // expanded = true => hiển thị panel
  panel.style.display = expanded ? "block" : "none";
  panel.hidden = !expanded;

  btn.setAttribute("aria-expanded", expanded ? "true" : "false");
  btn.classList.toggle("active", expanded);

  // Xoay icon
  if (icon) {
    if (rotateClass) {
      icon.classList.toggle(rotateClass, expanded);
    } else {
      icon.style.transform = expanded ? "rotate(180deg)" : "rotate(0deg)";
      icon.style.transition = "transform .2s ease";
    }
  }
}

function setupSearchClear(input, clearBtn) {
  // Hiển thị/ẩn nút clear theo giá trị input
  const sync = () => {
    clearBtn.style.display = input.value ? "flex" : "none";
  };
  sync();

  input.addEventListener("input", sync);

  clearBtn.addEventListener("click", () => {
    input.value = "";
    sync();
    input.focus();
    // Bắn event 'input' để các handler lọc/listen khác (nếu có) chạy lại
    input.dispatchEvent(new Event("input", { bubbles: true }));
  });
}

// Tự động khởi tạo với cấu hình mặc định
initUIEnhancements();
