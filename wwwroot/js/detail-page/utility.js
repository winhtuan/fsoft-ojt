// detail-page/utility.js
import { elements } from "./elements.js";
import { ENUM_LABELS, ENUM_ICONS } from "./constants.js";

export function getPlantIdFromUrl() {
  const pathParts = window.location.pathname.split("/");
  return pathParts[pathParts.length - 1];
}

export function showError(message) {
  elements.errorMessage.textContent = message;
  elements.errorAlert.classList.remove("d-none");
  elements.plantContent.classList.add("d-none");
}

export function hideError() {
  elements.errorAlert.classList.add("d-none");
}

export function getEnumLabel(enumType, value) {
  return ENUM_LABELS[enumType]?.[value] || "N/A";
}

export function getEnumIcon(enumType, value) {
  return ENUM_ICONS[enumType]?.[value] || "fa-circle";
}

export function createBadge(text, icon = "", type = "default") {
  const iconHtml = icon ? `<i class="fas ${icon} me-1"></i>` : "";
  let badgeClass = "badge me-2 mb-2";

  if (type === "plant-type") {
    const lowerText = text.toLowerCase();
    if (lowerText.includes("thân gỗ")) badgeClass += " plant-type-tree";
    else if (lowerText.includes("ăn quả")) badgeClass += " plant-type-fruit";
    else if (lowerText.includes("công nghiệp"))
      badgeClass += " plant-type-industry";
    else if (lowerText.includes("lương thực")) badgeClass += " plant-type-food";
    else if (lowerText.includes("cảnh")) badgeClass += " plant-type-ornamental";
    else if (lowerText.includes("rau") || lowerText.includes("gia vị"))
      badgeClass += " plant-type-vegetable";
    else badgeClass += " bg-success";
  } else if (type === "harvest") badgeClass += " harvest-date";
  else badgeClass += " bg-success";

  return `<span class="${badgeClass}">${iconHtml}${text}</span>`;
}

export function createQuickInfoCard(icon, label, value, cardClass = "") {
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

export function createCareItem(label, value) {
  return `
    <dt class="col-sm-4 mb-2">${label}</dt>
    <dd class="col-sm-8 mb-2 text-muted">${value}</dd>
  `;
}

export function createTagList(items, emptyMessage = "Chưa có thông tin") {
  if (!items || items.length === 0) {
    return `<p class="text-muted">${emptyMessage}</p>`;
  }
  const tags = items
    .map((item) => `<span class="badge me-2 mb-2 px-3 py-2">${item}</span>`)
    .join("");
  return `<div class="tag-list">${tags}</div>`;
}
