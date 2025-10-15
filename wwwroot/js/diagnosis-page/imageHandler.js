// diagnosis-page/imageHandler.js - Xử lý tất cả các thao tác liên quan đến ảnh
export class ImageHandler {
  constructor() {
    this.config = {
      acceptedTypes: ["image/jpeg", "image/png", "image/webp", "image/gif"],
      maxSizeMB: 8,
    };
  }

  /**
   * Validate file ảnh
   */
  validateImage(file) {
    // Kiểm tra file tồn tại
    if (!file || file.length === 0) {
      return {
        valid: false,
        error: "Bạn chưa chọn file!",
      };
    }

    // Kiểm tra loại file
    if (!this.config.acceptedTypes.includes(file.type)) {
      return {
        valid: false,
        error: "Vui lòng chọn ảnh JPG/PNG/WebP/GIF.",
      };
    }

    // Kiểm tra kích thước
    const maxBytes = this.config.maxSizeMB * 1024 * 1024;
    if (file.size > maxBytes) {
      const sizeMB = (file.size / (1024 * 1024)).toFixed(2);
      return {
        valid: false,
        error: `Ảnh quá lớn (${sizeMB} MB). Giới hạn ${this.config.maxSizeMB} MB.`,
      };
    }

    return { valid: true };
  }

  /**
   * Tạo preview ảnh
   */
  createPreview(file, previewElement) {
    return new Promise((resolve, reject) => {
      // Xóa nội dung cũ
      previewElement.innerHTML = "";
      previewElement.classList.remove("empty");

      // Tạo element img
      const img = document.createElement("img");
      const objectUrl = URL.createObjectURL(file);

      img.onload = () => {
        URL.revokeObjectURL(objectUrl);
        resolve(img);
      };

      img.onerror = () => {
        URL.revokeObjectURL(objectUrl);
        reject(new Error("Không thể tải ảnh"));
      };

      img.src = objectUrl;
      previewElement.appendChild(img);
    });
  }

  /**
   * Khởi tạo drag & drop cho một element
   */
  initDragAndDrop(element, onFileDrop) {
    // Ngăn chặn hành vi mặc định
    ["dragenter", "dragover", "dragleave", "drop"].forEach((eventName) => {
      element.addEventListener(eventName, (e) => {
        e.preventDefault();
        e.stopPropagation();
      });
    });

    // Thêm hiệu ứng khi drag over
    ["dragenter", "dragover"].forEach((eventName) => {
      element.addEventListener(eventName, () => {
        element.classList.add("drag-over");
      });
    });

    // Xóa hiệu ứng khi drag leave
    ["dragleave", "drop"].forEach((eventName) => {
      element.addEventListener(eventName, () => {
        element.classList.remove("drag-over");
      });
    });

    // Xử lý khi drop file
    element.addEventListener("drop", (e) => {
      const files = e.dataTransfer.files;
      if (files.length > 0 && onFileDrop) {
        onFileDrop(files[0]);
      }
    });
  }

  /**
   * Khởi tạo file input handler
   */
  initFileInput(inputElement, onFileSelect) {
    if (!inputElement) return;

    inputElement.addEventListener("change", () => {
      if (inputElement.files.length > 0 && onFileSelect) {
        onFileSelect(inputElement.files[0]);
      }
    });
  }

  /**
   * Resize ảnh nếu cần (tùy chọn - để tối ưu upload)
   */
  async resizeImage(file, maxWidth = 1920, maxHeight = 1920) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();

      reader.onload = (e) => {
        const img = new Image();

        img.onload = () => {
          const canvas = document.createElement("canvas");
          let width = img.width;
          let height = img.height;

          // Tính toán kích thước mới
          if (width > height) {
            if (width > maxWidth) {
              height *= maxWidth / width;
              width = maxWidth;
            }
          } else {
            if (height > maxHeight) {
              width *= maxHeight / height;
              height = maxHeight;
            }
          }

          canvas.width = width;
          canvas.height = height;

          const ctx = canvas.getContext("2d");
          ctx.drawImage(img, 0, 0, width, height);

          canvas.toBlob((blob) => {
            resolve(
              new File([blob], file.name, {
                type: file.type,
                lastModified: Date.now(),
              })
            );
          }, file.type);
        };

        img.onerror = () => reject(new Error("Không thể đọc ảnh"));
        img.src = e.target.result;
      };

      reader.onerror = () => reject(new Error("Không thể đọc file"));
      reader.readAsDataURL(file);
    });
  }

  /**
   * Lấy thông tin ảnh
   */
  getImageInfo(file) {
    return {
      name: file.name,
      size: file.size,
      sizeMB: (file.size / (1024 * 1024)).toFixed(2),
      type: file.type,
      lastModified: new Date(file.lastModified),
    };
  }
}
