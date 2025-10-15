// detail-page/render-functions.js
import { elements } from "./elements.js";
import {
  getEnumLabel,
  getEnumIcon,
  createBadge,
  createQuickInfoCard,
  createCareItem,
  createTagList,
} from "./utility.js";

export function renderBasicInfo(plant) {
  document.title = `${plant.commonName} - Plantpedia`;
  elements.breadcrumbName.textContent = plant.commonName;
  elements.plantCommonName.textContent = plant.commonName || "N/A";
  elements.plantScientificName.textContent = plant.scientificName || "N/A";
  elements.plantDescription.textContent =
    plant.description || "Chưa có mô tả chi tiết.";
}

export function renderBadges(plant) {
  let badgesHtml = "";
  if (plant.plantTypeName)
    badgesHtml += createBadge(plant.plantTypeName, "fa-leaf", "plant-type");
  if (plant.harvestDate)
    badgesHtml += createBadge(
      `${plant.harvestDate} ngày`,
      "fa-calendar-alt",
      "harvest"
    );
  elements.badgesContainer.innerHTML = badgesHtml;
}

export function renderImages(plant) {
  const images =
    plant.imageUrls && plant.imageUrls.length > 0
      ? plant.imageUrls
      : [plant.imageUrl].filter(Boolean);
  if (!images.length) return;
  elements.mainImage.src = images[0];
  elements.mainImage.alt = plant.commonName;

  if (images.length > 1) {
    const thumbnailsHtml = images
      .map(
        (url, index) => `
      <div class="col-3">
        <img src="${url}" class="img-thumbnail thumbnail-image ${
          index === 0 ? "active" : ""
        }" alt="Ảnh ${index + 1}" data-image-url="${url}">
      </div>
    `
      )
      .join("");
    elements.thumbnailContainer.innerHTML = thumbnailsHtml;

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

export function renderQuickInfo(plant) {
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
      createQuickInfoCard(info.icon, info.label, info.value, cardClasses[index])
    )
    .join("");
}

export function renderCareTab(plant) {
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

export function renderListTabs(plant) {
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

export function renderPlantData(plant, hideErrorFunc) {
  renderBasicInfo(plant);
  renderBadges(plant);
  renderImages(plant);
  renderQuickInfo(plant);
  renderCareTab(plant);
  renderListTabs(plant);
  hideErrorFunc();
  elements.plantContent.classList.remove("d-none");
}
