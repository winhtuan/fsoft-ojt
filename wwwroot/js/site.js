// site.js (SAFE lazy init for modalHelper)
window.modalHelper = (function () {
  let modalEl = null;
  let bsModal = null;

  function ensure() {
    if (modalEl && bsModal) return true;

    modalEl = document.getElementById("appModal");
    if (!modalEl) {
      // Thử lại sau khi DOM sẵn sàng (trường hợp partial render sau file này)
      document.addEventListener(
        "DOMContentLoaded",
        () => {
          if (!modalEl) modalEl = document.getElementById("appModal");
          if (modalEl && !bsModal) {
            bsModal = bootstrap.Modal.getOrCreateInstance(modalEl, {
              backdrop: "static",
              keyboard: false,
            });
          }
        },
        { once: true }
      );
      console.error("[ModalHelper] #appModal chưa có trong DOM.");
      return false;
    }

    bsModal = bootstrap.Modal.getOrCreateInstance(modalEl, {
      backdrop: "static",
      keyboard: false,
    });
    return true;
  }

  function setSize(size) {
    if (!ensure()) return;
    const dialog = modalEl.querySelector(".modal-dialog");
    dialog.classList.remove(
      "modal-sm",
      "modal-lg",
      "modal-xl",
      "modal-fullscreen"
    );
    if (size)
      dialog.classList.add(
        size === "full" ? "modal-fullscreen" : `modal-${size}`
      );
  }

  function show(options = {}) {
    if (!ensure()) return;

    const {
      title = "",
      body = "",
      footerHtml = null,
      onOk,
      onShown,
      onHidden,
      size,
    } = options;

    modalEl.querySelector(".modal-title").textContent = title;
    modalEl.querySelector(".modal-body").innerHTML = body;

    const footer = modalEl.querySelector(".modal-footer");
    if (footerHtml !== null) {
      footer.innerHTML = footerHtml;
    } else {
      footer.innerHTML = `
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
        <button type="button" class="btn btn-primary" id="appModalOkBtn">OK</button>`;
    }

    setSize(size);

    const okBtn = modalEl.querySelector("#appModalOkBtn");
    const okHandler = () => {
      try {
        if (typeof onOk === "function") onOk();
      } finally {
        bsModal.hide();
      }
    };
    if (okBtn) okBtn.onclick = okHandler;

    // Tránh nhân đôi listener giữa các lần show()
    modalEl.removeEventListener(
      "shown.bs.modal",
      modalEl._mh_shownHandler || (() => {})
    );
    modalEl.removeEventListener(
      "hidden.bs.modal",
      modalEl._mh_hiddenHandler || (() => {})
    );

    modalEl._mh_shownHandler = function () {
      if (onShown) onShown();
    };
    modalEl._mh_hiddenHandler = function () {
      if (onHidden) onHidden();
      modalEl.querySelector(".modal-body").innerHTML = "";
    };

    modalEl.addEventListener("shown.bs.modal", modalEl._mh_shownHandler);
    modalEl.addEventListener("hidden.bs.modal", modalEl._mh_hiddenHandler);

    bsModal.show();
    return bsModal;
  }

  function showConfirm(message, onOk) {
    return show({
      title: "Xác nhận",
      body: `<p>${message}</p>`,
      footerHtml: `
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
        <button type="button" class="btn btn-danger" id="appModalOkBtn">Xóa</button>`,
      onOk,
    });
  }

  function showAlert(message, title = "Thông báo") {
    return show({
      title,
      body: `<p>${message}</p>`,
      footerHtml: `<button type="button" class="btn text-white rounded-pill" id="appModalOkBtn" style="background-color: #28a745 !important;">OK</button>`,
    });
  }

  // tiện debug nhanh
  function _debug() {
    console.log({
      hasModalEl: !!document.getElementById("appModal"),
      initialized: !!bsModal,
      el: modalEl,
    });
  }

  return { show, showConfirm, showAlert, _debug };
})();
