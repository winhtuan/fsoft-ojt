// detail-page.js
(function () {
  "use strict";

  // ==================== CONSTANTS ====================
  const ENUM_LABELS = {
    LightRequirement: [
      "Ít ánh sáng",
      "Ánh sáng vừa",
      "Ánh sáng đầy đủ",
      "Ánh sáng mạnh",
    ],
    WateringNeeds: ["Thấp", "Trung bình", "Cao", "Rất cao"],
    HumidityPreference: ["Thấp", "Trung bình", "Cao"],
    GrowthRate: ["Chậm", "Trung bình", "Nhanh"],
  };

  const ENUM_ICONS = {
    LightRequirement: ["fa-moon", "fa-cloud-sun", "fa-sun", "fa-sun"],
    WateringNeeds: ["fa-tint", "fa-tint", "fa-tint", "fa-tint"],
    HumidityPreference: ["fa-droplet", "fa-droplet", "fa-droplet"],
    GrowthRate: ["fa-seedling", "fa-chart-line", "fa-rocket"],
  };

  // ==================== DOM ELEMENTS ====================
  const elements = {
    errorAlert: document.getElementById("errorAlert"),
    errorMessage: document.getElementById("errorMessage"),
    plantContent: document.getElementById("plantContent"),
    breadcrumbName: document.getElementById("breadcrumbName"),
    mainImage: document.getElementById("mainImage"),
    thumbnailContainer: document.getElementById("thumbnailContainer"),
    plantCommonName: document.getElementById("plantCommonName"),
    plantScientificName: document.getElementById("plantScientificName"),
    badgesContainer: document.getElementById("badgesContainer"),
    plantDescription: document.getElementById("plantDescription"),
    quickInfoContainer: document.getElementById("quickInfoContainer"),
    careList: document.getElementById("careList"),
    regionContent: document.getElementById("regionContent"),
    soilContent: document.getElementById("soilContent"),
    climateContent: document.getElementById("climateContent"),
    usageContent: document.getElementById("usageContent"),
  };

  // ==================== UTILITY FUNCTIONS ====================
  function getPlantIdFromUrl() {
    const pathParts = window.location.pathname.split("/");
    return pathParts[pathParts.length - 1];
  }

  function showError(message) {
    elements.errorMessage.textContent = message;
    elements.errorAlert.classList.remove("d-none");
    elements.plantContent.classList.add("d-none");
  }

  function hideError() {
    elements.errorAlert.classList.add("d-none");
  }

  function getEnumLabel(enumType, value) {
    return ENUM_LABELS[enumType]?.[value] || "N/A";
  }

  function getEnumIcon(enumType, value) {
    return ENUM_ICONS[enumType]?.[value] || "fa-circle";
  }

  function createBadge(text, icon = "", type = "default") {
    const iconHtml = icon ? `<i class="fas ${icon} me-1"></i>` : "";
    let badgeClass = "badge me-2 mb-2";

    if (type === "plant-type") {
      const lowerText = text.toLowerCase();

      if (lowerText.includes("thân gỗ")) {
        badgeClass += " plant-type-tree";
      } else if (lowerText.includes("ăn quả")) {
        badgeClass += " plant-type-fruit";
      } else if (lowerText.includes("công nghiệp")) {
        badgeClass += " plant-type-industry";
      } else if (lowerText.includes("lương thực")) {
        badgeClass += " plant-type-food";
      } else if (lowerText.includes("cảnh")) {
        badgeClass += " plant-type-ornamental";
      } else if (lowerText.includes("rau") || lowerText.includes("gia vị")) {
        badgeClass += " plant-type-vegetable";
      } else {
        badgeClass += " bg-success";
      }
    } else if (type === "harvest") {
      badgeClass += " harvest-date";
    } else {
      badgeClass += " bg-success";
    }

    return `<span class="${badgeClass}">${iconHtml}${text}</span>`;
  }

  function createQuickInfoCard(icon, label, value, cardClass = "") {
    return `
            <div class="col-md-6 mb-3">
                <div class="quick-info-card ${cardClass} p-3 rounded bg-light h-100">
                    <div class="d-flex align-items-center">
                        <i class="fas ${icon} fa-2x me-3"></i>
                        <div>
                            <small class="text-muted d-block">${label}</small>
                            <strong class="text-dark">${value}</strong>
                        </div>
                    </div>
                </div>
            </div>
        `;
  }

  function createCareItem(label, value) {
    return `
            <dt class="col-sm-4 mb-2">${label}</dt>
            <dd class="col-sm-8 mb-2 text-muted">${value}</dd>
        `;
  }

  function createTagList(items, emptyMessage = "Chưa có thông tin") {
    if (!items || items.length === 0) {
      return `<p class="text-muted">${emptyMessage}</p>`;
    }

    const tags = items
      .map((item) => `<span class="badge me-2 mb-2 px-3 py-2">${item}</span>`)
      .join("");

    return `<div class="tag-list">${tags}</div>`;
  }

  // ==================== RENDER FUNCTIONS ====================
  function renderBasicInfo(plant) {
    // Update title and breadcrumb
    document.title = `${plant.commonName} - Plantpedia`;
    elements.breadcrumbName.textContent = plant.commonName;

    // Update basic info
    elements.plantCommonName.textContent = plant.commonName || "N/A";
    elements.plantScientificName.textContent = plant.scientificName || "N/A";
    elements.plantDescription.textContent =
      plant.description || "Chưa có mô tả chi tiết.";
  }

  function renderBadges(plant) {
    let badgesHtml = "";

    if (plant.plantTypeName) {
      badgesHtml += createBadge(plant.plantTypeName, "fa-leaf", "plant-type");
    }

    if (plant.harvestDate) {
      badgesHtml += createBadge(
        `${plant.harvestDate} ngày`,
        "fa-calendar-alt",
        "harvest"
      );
    }

    elements.badgesContainer.innerHTML = badgesHtml;
  }

  function renderImages(plant) {
    const images =
      plant.imageUrls && plant.imageUrls.length > 0
        ? plant.imageUrls
        : [plant.imageUrl].filter(Boolean);

    if (images.length === 0) {
      return;
    }

    // Set main image
    elements.mainImage.src = images[0];
    elements.mainImage.alt = plant.commonName;

    // Render thumbnails
    if (images.length > 1) {
      const thumbnailsHtml = images
        .map(
          (url, index) => `
                <div class="col-3">
                    <img src="${url}" 
                         class="img-thumbnail thumbnail-image ${
                           index === 0 ? "active" : ""
                         }" 
                         alt="Ảnh ${index + 1}"
                         data-image-url="${url}">
                </div>
            `
        )
        .join("");

      elements.thumbnailContainer.innerHTML = thumbnailsHtml;

      // Add click handlers
      elements.thumbnailContainer
        .querySelectorAll(".thumbnail-image")
        .forEach((thumb) => {
          thumb.addEventListener("click", function () {
            elements.mainImage.src = this.dataset.imageUrl;
            elements.thumbnailContainer
              .querySelectorAll(".thumbnail-image")
              .forEach((t) => t.classList.remove("active"));
            this.classList.add("active");
          });
        });
    }
  }

  function renderQuickInfo(plant) {
    const quickInfo = [
      {
        icon: getEnumIcon("LightRequirement", plant.lightRequirement),
        label: "Yêu cầu ánh sáng",
        value: getEnumLabel("LightRequirement", plant.lightRequirement),
      },
      {
        icon: getEnumIcon("WateringNeeds", plant.wateringNeeds),
        label: "Nhu cầu tưới nước",
        value: getEnumLabel("WateringNeeds", plant.wateringNeeds),
      },
      {
        icon: getEnumIcon("HumidityPreference", plant.humidityPreference),
        label: "Độ ẩm",
        value: getEnumLabel("HumidityPreference", plant.humidityPreference),
      },
      {
        icon: getEnumIcon("GrowthRate", plant.growthRate),
        label: "Tốc độ phát triển",
        value: getEnumLabel("GrowthRate", plant.growthRate),
      },
    ];

    const cardClasses = [
      "light-card",
      "water-card",
      "humidity-card",
      "growth-card",
    ];

    elements.quickInfoContainer.innerHTML = quickInfo
      .map((info, index) =>
        createQuickInfoCard(
          info.icon,
          info.label,
          info.value,
          cardClasses[index]
        )
      )
      .join("");
  }

  function renderCareTab(plant) {
    const careItems = [
      {
        label: "Yêu cầu ánh sáng",
        value: getEnumLabel("LightRequirement", plant.lightRequirement),
      },
      {
        label: "Nhu cầu tưới nước",
        value: getEnumLabel("WateringNeeds", plant.wateringNeeds),
      },
      {
        label: "Độ ẩm",
        value: getEnumLabel("HumidityPreference", plant.humidityPreference),
      },
      {
        label: "Tốc độ phát triển",
        value: getEnumLabel("GrowthRate", plant.growthRate),
      },
      {
        label: "Khuyến nghị về đất",
        value: plant.soilRecommendation || "Chưa có thông tin",
      },
      {
        label: "Thông tin phân bón",
        value: plant.fertilizerInfo || "Chưa có thông tin",
      },
    ];

    elements.careList.innerHTML = careItems
      .map((item) => createCareItem(item.label, item.value))
      .join("");
  }

  function renderListTabs(plant) {
    elements.regionContent.innerHTML = createTagList(
      plant.regionNames,
      "Chưa có thông tin về vùng trồng phù hợp."
    );

    elements.soilContent.innerHTML = createTagList(
      plant.soilNames,
      "Chưa có thông tin về loại đất phù hợp."
    );

    elements.climateContent.innerHTML = createTagList(
      plant.climateNames,
      "Chưa có thông tin về khí hậu phù hợp."
    );

    elements.usageContent.innerHTML = createTagList(
      plant.usageNames,
      "Chưa có thông tin về mục đích sử dụng."
    );
  }

  function renderPlantData(plant) {
    renderBasicInfo(plant);
    renderBadges(plant);
    renderImages(plant);
    renderQuickInfo(plant);
    renderCareTab(plant);
    renderListTabs(plant);

    // Show content
    hideError();
    elements.plantContent.classList.remove("d-none");
  }

  // ==================== API FUNCTIONS ====================
  async function fetchPlantData(plantId) {
    try {
      const response = await fetch(`/api/Plant/${plantId}`);

      if (!response.ok) {
        if (response.status === 404) {
          throw new Error("Không tìm thấy thông tin cây trồng này.");
        } else if (response.status === 400) {
          throw new Error("Yêu cầu không hợp lệ.");
        } else {
          throw new Error("Có lỗi xảy ra khi tải dữ liệu.");
        }
      }

      const data = await response.json();
      return data;
    } catch (error) {
      if (error.message) {
        throw error;
      }
      throw new Error(
        "Không thể kết nối đến máy chủ. Vui lòng kiểm tra kết nối internet."
      );
    }
  }

  // ==================== INITIALIZATION ====================
  async function init() {
    const plantId = getPlantIdFromUrl();

    if (!plantId) {
      showError("ID cây trồng không hợp lệ.");
      return;
    }

    try {
      const plant = await fetchPlantData(plantId);
      renderPlantData(plant);
    } catch (error) {
      console.error("Error loading plant data:", error);
      showError(error.message);
    }
  }

  // ==================== EVENT LISTENERS ====================
  document.addEventListener("DOMContentLoaded", init);
})();
