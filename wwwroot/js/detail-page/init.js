// detail-page/init.js
import { getPlantIdFromUrl, showError, hideError } from "./utility.js";
import { fetchPlantData } from "./api-functions.js";
import { renderPlantData } from "./render-functions.js";

export async function init() {
  const plantId = getPlantIdFromUrl();
  if (!plantId) {
    showError("ID cây trồng không hợp lệ.");
    return;
  }

  try {
    const plant = await fetchPlantData(plantId);
    renderPlantData(plant, hideError);
  } catch (error) {
    console.error("Error loading plant data:", error);
    showError(error.message);
  }
}
