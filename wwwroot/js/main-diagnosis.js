// main-diagnosis.js - File chính import và khởi tạo
import { DiagnosisApi } from "./diagnosis-page/diagnosisApi.js";
import { ImageHandler } from "./diagnosis-page/imageHandler.js";

class DiagnosisManager {
  constructor() {
    this.api = new DiagnosisApi();
    this.imageHandler = new ImageHandler();
    this.elements = {
      imageInput: null,
      previewDiv: null,
      resultDiv: null,
      errorDiv: null,
    };
    this.isBusy = false;
  }

  /**
   * Khởi tạo ứng dụng
   */
  init() {
    // Lấy các elements
    this.elements.imageInput = document.getElementById("image-input");
    this.elements.previewDiv = document.getElementById("preview-image");
    this.elements.resultDiv = document.getElementById("diagnosis-result");
    this.elements.errorDiv = document.getElementById("diagnosis-error");

    // Kiểm tra elements có tồn tại
    if (
      !this.elements.previewDiv ||
      !this.elements.resultDiv ||
      !this.elements.errorDiv
    ) {
      console.error("Không tìm thấy các elements cần thiết!");
      return;
    }

    // Khởi tạo drag & drop
    this.imageHandler.initDragAndDrop(this.elements.previewDiv, (file) => {
      this.handleFile(file);
    });

    // Khởi tạo file input
    this.imageHandler.initFileInput(this.elements.imageInput, (file) => {
      this.handleFile(file);
    });

    console.log("DiagnosisManager đã khởi tạo thành công");
  }

  /**
   * Xử lý file được chọn
   */
  async handleFile(file) {
    // Validate ảnh
    const validation = this.imageHandler.validateImage(file);
    if (!validation.valid) {
      this.showError(validation.error);
      return;
    }

    // Log thông tin file
    const info = this.imageHandler.getImageInfo(file);
    console.log("File info:", info);

    // Hiển thị preview
    this.clearError();
    try {
      await this.imageHandler.createPreview(file, this.elements.previewDiv);
    } catch (error) {
      this.showError("Không thể hiển thị ảnh preview");
      return;
    }

    // Upload và chẩn đoán
    await this.diagnose(file);
  }

  /**
   * Thực hiện chẩn đoán
   */
  async diagnose(file) {
    // Chặn nếu đang xử lý
    if (this.isBusy) return;

    this.isBusy = true;
    this.showLoading();

    try {
      // Gọi API
      const rawData = await this.api.diagnose(file);
      console.log("Raw API response:", rawData);

      // Parse kết quả
      const parsedResult = this.api.parseResult(rawData);

      if (!parsedResult.success) {
        this.showError(parsedResult.message);
        return;
      }

      // Hiển thị kết quả
      this.displayResult(parsedResult.data);
    } catch (error) {
      console.error("Diagnosis error:", error);
      this.showError(error.message || "Đã xảy ra lỗi khi xử lý yêu cầu!");
    } finally {
      this.isBusy = false;
    }
  }

  /**
   * Hiển thị kết quả chẩn đoán
   */
  displayResult(data) {
    let html = "";

    // Header trạng thái sức khỏe
    if (data.isHealthy) {
      const alertClass = data.isHealthy.healthy
        ? "alert-info"
        : "alert-warning";
      html += `<div class="alert ${alertClass} mb-3">${this.escapeHtml(
        data.isHealthy.message
      )}</div>`;
    }

    // Thông tin loài
    if (data.species) {
      html += this.renderSpecies(data.species);
    }

    // Danh sách bệnh
    html += this.renderDiseases(data.diseases);

    this.elements.resultDiv.innerHTML = html;
  }

  /**
   * Render thông tin loài
   */
  renderSpecies(species) {
    let html = `
      <div class="section-title">Nhận diện loài</div>
      <div><b>Tên khoa học:</b> ${this.escapeHtml(species.scientificName)}</div>
      <div><b>Tên phổ biến:</b> ${this.escapeHtml(species.commonName)}</div>`;

    if (species.probability != null) {
      html += `<div class="mt-1"><span class="badge">${species.probability}%</span></div>`;
    }

    // Các gợi ý khác
    if (species.alternatives && species.alternatives.length > 0) {
      html += `<div class="mt-2 muted">Gợi ý khác:</div><ul class="small">`;
      species.alternatives.forEach((alt) => {
        html += `<li>${this.escapeHtml(alt.name)} <span class="badge ms-2">${
          alt.probability ?? "?"
        }%</span></li>`;
      });
      html += `</ul>`;
    }

    html += `<hr/>`;
    return html;
  }

  /**
   * Render danh sách bệnh
   */
  renderDiseases(diseases) {
    if (!diseases || diseases.length === 0) {
      return `
        <div class="section-title">Chẩn đoán bệnh</div>
        <div class="alert alert-info mb-0">Chưa phát hiện bệnh rõ ràng từ ảnh này.</div>`;
    }

    let html = `<div class="section-title">Chẩn đoán bệnh (tham khảo)</div>`;

    diseases.forEach((disease) => {
      html += `
        <div class="disease">
          <div class="name">
            ${this.escapeHtml(disease.name)}
            ${
              disease.probability != null
                ? `<span class="badge bad ms-2">${disease.probability}%</span>`
                : ""
            }
          </div>
          ${
            disease.description
              ? `<div class="muted mt-1">${this.escapeHtml(
                  disease.description
                )}</div>`
              : ""
          }
          ${
            disease.treatment
              ? `<div class="mt-1">${this.escapeHtml(disease.treatment)}</div>`
              : ""
          }
          ${
            disease.probability != null
              ? this.createProgressBar(disease.probability)
              : ""
          }
        </div>`;
    });

    return html;
  }

  /**
   * Tạo progress bar
   */
  createProgressBar(percentage) {
    const pct = Math.max(0, Math.min(100, percentage));
    return `
      <div class="progress"><span style="width:${pct}%"></span></div>
      <div class="muted mt-1">${pct}%</div>`;
  }

  /**
   * Hiển thị loading
   */
  showLoading() {
    this.elements.resultDiv.innerHTML = `
      <div class="d-inline-flex align-items-center gap-2">
        <div class="spinner-border text-success" role="status" style="width:1.25rem;height:1.25rem;"></div>
        <span>Đang phân tích...</span>
      </div>`;
  }

  /**
   * Hiển thị lỗi
   */
  showError(message) {
    this.elements.errorDiv.textContent = message;
    this.elements.errorDiv.classList.remove("d-none");
    this.elements.resultDiv.innerHTML = "";
  }

  /**
   * Xóa thông báo lỗi
   */
  clearError() {
    this.elements.errorDiv.classList.add("d-none");
    this.elements.errorDiv.textContent = "";
  }

  /**
   * Escape HTML để tránh XSS
   */
  escapeHtml(text) {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
  }
}

// Khởi tạo khi DOM ready
document.addEventListener("DOMContentLoaded", function () {
  const manager = new DiagnosisManager();
  manager.init();
});
