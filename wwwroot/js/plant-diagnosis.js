document.addEventListener("DOMContentLoaded", function () {
  const imageInput = document.getElementById("image-input");
  const previewDiv = document.getElementById("preview-image");
  const resultDiv = document.getElementById("diagnosis-result");
  const errorDiv = document.getElementById("diagnosis-error");

  // ==== CONFIG NHANH ====
  const ACCEPTED_TYPES = ["image/jpeg", "image/png", "image/webp", "image/gif"];
  const MAX_MB = 8; // giới hạn kích thước ảnh
  const TOP_DISEASES = 5; // hiển thị tối đa n bệnh
  let busy = false; // chặn double upload

  // ==== HELPERS ====
  const mb = (bytes) => (bytes / (1024 * 1024)).toFixed(2);

  // Từ điển dịch tên bệnh sang tiếng Việt
  const diseaseNameMap = {
    "nutrient deficiency": "Thiếu dinh dưỡng",
    fungi: "Nấm bệnh",
    "fungal disease": "Bệnh nấm",
    "bacterial disease": "Bệnh vi khuẩn",
    "viral disease": "Bệnh virus",
    "pest damage": "Sâu bệnh hại",
    "insect damage": "Côn trùng gây hại",
    "powdery mildew": "Phấn trắng",
    "leaf spot": "Đốm lá",
    rust: "Bệnh gỉ sắt",
    blight: "Bệnh khô héo",
    "root rot": "Thối rễ",
    wilt: "Bệnh héo",
    "mosaic virus": "Virus khảm lá",
    anthracnose: "Bệnh thán thư",
    "downy mildew": "Sương mai",
    "black spot": "Đốm đen",
    canker: "Bệnh loét thân",
    scab: "Bệnh ghẻ",
    "fire blight": "Bệnh cháy lá",
    chlorosis: "Vàng lá",
    necrosis: "Hoại tử mô",
    sunburn: "Cháy nắng",
    "frost damage": "Hư hại do sương giá",
    overwatering: "Tưới nước quá nhiều",
    underwatering: "Thiếu nước",
    "nitrogen deficiency": "Thiếu đạm",
    "phosphorus deficiency": "Thiếu phốt pho",
    "potassium deficiency": "Thiếu kali",
    "iron deficiency": "Thiếu sắt",
    "magnesium deficiency": "Thiếu magiê",
    "calcium deficiency": "Thiếu canxi",
    "early blight": "Bệnh sương mai sớm",
    "late blight": "Bệnh sương mai muộn",
    "septoria leaf spot": "Đốm lá septoria",
    "gray mold": "Nấm xám",
    "brown rot": "Thối nâu",
    "sooty mold": "Nấm bồ hóng",
    "fusarium wilt": "Héo fusarium",
    "verticillium wilt": "Héo verticillium",
    "bacterial spot": "Đốm vi khuẩn",
    "bacterial blight": "Cháy lá vi khuẩn",
    "crown rot": "Thối cổ rễ",
    "damping off": "Bệnh ngã rạp",
    "leaf curl": "Cuộn lá",
    "yellow leaf": "Vàng lá",
    "brown leaf": "Lá nâu",
    "tip burn": "Cháy đầu lá",
    "edge burn": "Cháy viền lá",
  };

  function translateDiseaseName(englishName) {
    if (!englishName) return "Không rõ";
    const lowerName = englishName.toLowerCase().trim();
    return diseaseNameMap[lowerName] || englishName;
  }

  function showError(msg) {
    errorDiv.textContent = msg;
    errorDiv.classList.remove("d-none");
    resultDiv.innerHTML = "";
  }

  function clearError() {
    errorDiv.classList.add("d-none");
    errorDiv.textContent = "";
  }

  function progressBar(percentage) {
    const pct = Math.max(0, Math.min(100, Number(percentage) || 0));
    return `
      <div class="progress"><span style="width:${pct}%"></span></div>
      <div class="muted mt-1">${pct}%</div>
    `;
  }

  function setLoading() {
    resultDiv.innerHTML = `
      <div class="d-inline-flex align-items-center gap-2">
        <div class="spinner-border text-success" role="status" style="width:1.25rem;height:1.25rem;"></div>
        <span>Đang phân tích...</span>
      </div>`;
  }

  // ==== DRAG & DROP (tùy chọn, dùng luôn vùng preview) ====
  ["dragenter", "dragover"].forEach((evt) =>
    previewDiv.addEventListener(evt, (e) => {
      e.preventDefault();
      e.stopPropagation();
      previewDiv.classList.add("drag-over");
    })
  );
  ["dragleave", "drop"].forEach((evt) =>
    previewDiv.addEventListener(evt, (e) => {
      e.preventDefault();
      e.stopPropagation();
      previewDiv.classList.remove("drag-over");
    })
  );
  previewDiv.addEventListener("drop", (e) => {
    const file = e.dataTransfer?.files?.[0];
    if (file) handleFile(file);
  });

  // ==== FILE INPUT ====
  if (imageInput) {
    imageInput.addEventListener("change", function () {
      if (imageInput.files.length > 0) {
        handleFile(imageInput.files[0]);
      }
    });
  }

  function handleFile(file) {
    // Validate
    if (!ACCEPTED_TYPES.includes(file.type)) {
      showError("Vui lòng chọn ảnh JPG/PNG/WebP/GIF.");
      return;
    }
    if (file.size > MAX_MB * 1024 * 1024) {
      showError(`Ảnh quá lớn (${mb(file.size)} MB). Giới hạn ${MAX_MB} MB.`);
      return;
    }

    // Preview
    clearError();
    previewDiv.innerHTML = "";
    previewDiv.classList.remove("empty");
    const img = document.createElement("img");
    img.src = URL.createObjectURL(file);
    img.onload = () => URL.revokeObjectURL(img.src);
    previewDiv.appendChild(img);

    uploadImage(file);
  }

  async function uploadImage(file) {
    if (!file || busy) return;
    busy = true;
    clearError();
    setLoading();

    const formData = new FormData();
    // Client gửi field "image" tới server;
    // Service server sẽ chuyển sang field "images" khi gọi Plant.id v3
    formData.append("image", file);

    try {
      const resp = await fetch("/api/PlantDiagnosis/detect", {
        method: "POST",
        body: formData,
      });

      let json;
      try {
        json = await resp.json();
        // console.log("DEBUG raw result:", json);
      } catch {
        showError("Lỗi định dạng dữ liệu trả về từ server!");
        busy = false;
        return;
      }

      if (!resp.ok || json.success === false) {
        showError(json.message || "Đã xảy ra lỗi khi chẩn đoán!");
        busy = false;
        return;
      }

      showResult(json.data || json);
    } catch (e) {
      showError("Không thể gửi ảnh đến server!");
    } finally {
      busy = false;
    }
  }

  function showResult(data) {
    // Một số trường hợp API có thể trả mảng (nhiều ảnh)
    if (Array.isArray(data)) data = data[0];

    const r = data?.result || data; // tùy server có wrap hay không
    if (!r) {
      resultDiv.innerHTML = `<div class="alert alert-warning">Không có dữ liệu kết quả.</div>`;
      return;
    }

    // ==== Đánh giá là thực vật? (v3 & v5) ====
    let isPlant = null,
      plantConf = null;

    if (typeof r.is_plant === "boolean") {
      isPlant = r.is_plant;
    } else if (r.is_plant && typeof r.is_plant === "object") {
      isPlant = r.is_plant.binary === true;
      plantConf = r.is_plant.confidence ?? null;
    } else if (typeof r.is_plant_probability === "number") {
      plantConf = r.is_plant_probability;
      if (isPlant == null) isPlant = plantConf >= 0.5;
    } else if (r.is_healthy !== undefined) {
      // Một số payload health_assessment không gửi is_plant rõ ràng
      isPlant = true;
      plantConf = null;
    }

    if (isPlant !== true) {
      resultDiv.innerHTML = `<div class="alert alert-warning">Không nhận diện được cây trồng.</div>`;
      return;
    }

    // ==== Header trạng thái tổng quát ====
    let header = "";
    if (r.is_healthy?.binary === true) {
      header += `<div class="alert alert-info mb-3">Cây có vẻ khỏe mạnh tổng thể.</div>`;
    } else if (r.is_healthy?.binary === false) {
      header += `<div class="alert alert-warning mb-3">Cây có dấu hiệu không khỏe.</div>`;
    }

    // ==== Nhận diện loài (nếu có – health=only sẽ không có) ====
    let identity = "";
    const suggestions = r.classification?.suggestions || r.suggestions || [];
    if (suggestions.length > 0) {
      const top = suggestions[0];
      const sci =
        top.details?.scientific_name || top.scientific_name || "(không có)";
      const common =
        (top.details?.common_names && top.details.common_names[0]) ||
        top.name ||
        "(không có)";
      const prob =
        top.probability != null ? Math.round(top.probability * 100) : null;

      identity += `
        <div class="section-title">Nhận diện loài</div>
        <div><b>Tên khoa học:</b> ${sci}</div>
        <div><b>Tên phổ biến:</b> ${common}</div>
        ${
          prob != null
            ? `<div class="mt-1"><span class="badge">${prob}%</span></div>`
            : ""
        }`;

      if (suggestions.length > 1) {
        identity += `<div class="mt-2 muted">Gợi ý khác:</div><ul class="small">`;
        suggestions.slice(1, 4).forEach((s) => {
          const nm =
            s.details?.scientific_name || s.scientific_name || s.name || "N/A";
          const p2 =
            s.probability != null ? Math.round(s.probability * 100) : "?";
          identity += `<li>${nm} <span class="badge ms-2">${p2}%</span></li>`;
        });
        identity += `</ul>`;
      }
      identity += `<hr/>`;
    }

    // ==== Chẩn đoán bệnh (v3 & v5; hỗ trợ nhiều cấu trúc field) ====
    let diseases =
      r.disease?.suggestions || // Plant.id v3 health_assessment phổ biến
      r.health_assessment?.diseases ||
      r.diseases ||
      data?.health_assessment?.diseases ||
      [];

    let diseasesHtml = "";
    if (diseases.length > 0) {
      diseases = diseases
        .slice()
        .sort((a, b) => (b.probability ?? 0) - (a.probability ?? 0))
        .slice(0, TOP_DISEASES);

      diseasesHtml += `<div class="section-title">Chẩn đoán bệnh (tham khảo)</div>`;
      diseases.forEach((d) => {
        const name = d.name || d.disease || "Không rõ";
        const translatedName = translateDiseaseName(name);
        const p =
          d.probability != null ? Math.round(d.probability * 100) : null;

        // Mô tả & khuyến nghị có thể nằm ở nhiều nhánh tuỳ phiên bản schema:
        const desc =
          d.details?.description ||
          d.disease_details?.description ||
          d.description ||
          "";

        const rec =
          d.details?.treatment ||
          d.recommendation ||
          d.disease_details?.treatment ||
          "";

        diseasesHtml += `
          <div class="disease">
            <div class="name">
              ${translatedName}
              ${p != null ? `<span class="badge bad ms-2">${p}%</span>` : ""}
            </div>
            ${desc ? `<div class="muted mt-1">${desc}</div>` : ""}
            ${rec ? `<div class="mt-1">${rec}</div>` : ""}
            ${p != null ? progressBar(p) : ""}
          </div>`;
      });
    } else {
      diseasesHtml += `<div class="section-title">Chẩn đoán bệnh</div>
        <div class="alert alert-info mb-0">Chưa phát hiện bệnh rõ ràng từ ảnh này.</div>`;
    }

    // ==== Đổ HTML vào panel-right ====
    resultDiv.innerHTML = `${header}${identity}${diseasesHtml}`;
  }
});
