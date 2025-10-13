document.addEventListener("DOMContentLoaded", function () {
  const forgotBtn = document.getElementById("openForgot");
  const modalEl = document.getElementById("forgotModal");
  const modal = new bootstrap.Modal(modalEl);

  const stepEmail = document.getElementById("fpStepEmail");
  const stepCode = document.getElementById("fpStepCode");
  const fpEmail = document.getElementById("fpEmail");
  const fpCode = document.getElementById("fpCode");
  const fpNewPass = document.getElementById("fpNewPassword");
  const fpAlert = document.getElementById("fpAlert");

  const btnSend = document.getElementById("btnSendCode");
  const btnReset = document.getElementById("btnResetPassword");
  const btnResend = document.getElementById("btnResend");

  function showAlert(msg, type = "info") {
    fpAlert.innerHTML = `<div class="alert alert-${type} mb-0">${msg}</div>`;
  }
  function clearAlert() {
    fpAlert.innerHTML = "";
  }

  function toStep(step) {
    if (step === 1) {
      stepEmail.classList.remove("d-none");
      stepCode.classList.add("d-none");
    } else {
      stepEmail.classList.add("d-none");
      stepCode.classList.remove("d-none");
    }
    clearAlert();
  }

  forgotBtn?.addEventListener("click", (e) => {
    e.preventDefault();
    toStep(1);
    fpEmail.value = "";
    fpCode.value = "";
    fpNewPass.value = "";
    clearAlert();
    modal.show();
  });

  async function postForm(url, data) {
    const formData = new FormData();
    Object.entries(data).forEach(([k, v]) => formData.append(k, v));
    const resp = await fetch(url, { method: "POST", body: formData });
    return await resp.json();
  }

  btnSend.addEventListener("click", async () => {
    clearAlert();
    const email = fpEmail.value.trim();
    if (!email) return showAlert("Vui lòng nhập email.", "warning");

    btnSend.disabled = true;
    btnSend.textContent = "Đang gửi...";
    try {
      const res = await postForm("/auth/forgot/send-code", { email });
      // Luôn báo chung để tránh lộ thông tin
      showAlert("Nếu email tồn tại, mã xác nhận đã được gửi.", "success");
      toStep(2);
    } catch {
      showAlert("Không thể gửi yêu cầu. Vui lòng thử lại.", "danger");
    } finally {
      btnSend.disabled = false;
      btnSend.textContent = "Gửi mã xác nhận";
    }
  });

  btnResend.addEventListener("click", async (e) => {
    e.preventDefault();
    clearAlert();
    const email = fpEmail.value.trim();
    if (!email) return showAlert("Vui lòng nhập email.", "warning");
    try {
      await postForm("/auth/forgot/send-code", { email });
      showAlert("Đã gửi lại mã xác nhận (nếu email hợp lệ).", "success");
    } catch {
      showAlert("Không thể gửi lại mã. Vui lòng thử lại.", "danger");
    }
  });

  btnReset.addEventListener("click", async () => {
    clearAlert();
    const email = fpEmail.value.trim();
    const code = fpCode.value.trim();
    const pass = fpNewPass.value;

    if (!code || code.length !== 6)
      return showAlert("Mã xác nhận gồm 6 số.", "warning");
    if (!pass || pass.length < 6)
      return showAlert("Mật khẩu tối thiểu 6 ký tự.", "warning");

    btnReset.disabled = true;
    btnReset.textContent = "Đang đổi mật khẩu...";
    try {
      const res = await postForm("/auth/forgot/reset", {
        email,
        code,
        newPassword: pass,
      });
      if (res.success) {
        showAlert(
          "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.",
          "success"
        );
        setTimeout(() => {
          modal.hide();
          location.reload();
        }, 1200);
      } else {
        showAlert(res.message || "Mã không hợp lệ hoặc đã hết hạn.", "danger");
      }
    } catch {
      showAlert("Không thể đổi mật khẩu. Vui lòng thử lại.", "danger");
    } finally {
      btnReset.disabled = false;
      btnReset.textContent = "Đổi mật khẩu";
    }
  });
});
