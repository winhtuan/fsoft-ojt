import { fetchChartData } from "./report-page/api.js";
import {
  ChartManager,
  setChartToggles,
  setCategoryText,
  setSpinner,
  updateExportInputs,
  renderTableAndKpis,
} from "./report-page/chart-ui.js";
// Lưu ý: setChartToggles, … được export trong chart-ui.js; nếu IDE báo not found, hãy export thêm như dưới:
export {
  setChartToggles,
  setCategoryText,
  setSpinner,
  updateExportInputs,
  renderTableAndKpis,
} from "./report-page/chart-ui.js";

document.addEventListener("DOMContentLoaded", async () => {
  "use strict";

  let currentType = "plantType";
  let currentDate = null;

  const ctx = document.getElementById("mainChart")?.getContext("2d");
  const datePickerEl = document.getElementById("datePicker");
  if (!ctx) {
    console.error("[Report] Missing #mainChart");
    return;
  }

  // Chart
  const chartMgr = new ChartManager(ctx, "bar");

  // Datepicker (dynamic import để giảm tải ban đầu)
  if (datePickerEl) {
    const { setupDatePicker } = await import("./report-page/datepicker.js");
    setupDatePicker(datePickerEl, {
      onApply: async (d) => {
        currentDate = d;
        await loadAndRender();
        updateExportInputs(currentType, currentDate);
      },
    });
  }

  async function loadAndRender() {
    setSpinner(true);
    try {
      const json = await fetchChartData(currentType, currentDate);
      chartMgr.updateFromJson(json, currentType);
      renderTableAndKpis(json);
    } catch (e) {
      console.error("Failed to load chart data:", e);
    } finally {
      setSpinner(false);
    }
  }

  // Category (jQuery)
  $(".category-option").on("click", function (e) {
    e.preventDefault();
    currentType = $(this).data("category");
    setCategoryText($(this).text().trim());
    void loadAndRender();
    updateExportInputs(currentType, currentDate);
  });
  const initialText =
    $(`.category-option[data-category='${currentType}']`).text()?.trim() ||
    "Theo danh mục";
  setCategoryText(initialText);

  // Chart toggles
  document.getElementById("barToggle")?.addEventListener("click", () => {
    chartMgr.swap("bar");
  });
  document.getElementById("pieToggle")?.addEventListener("click", () => {
    chartMgr.swap("pie");
  });

  // Reset
  document.getElementById("resetFilters")?.addEventListener("click", () => {
    currentType = "plantType";
    currentDate = null;
    const dp = document.getElementById("datePicker");
    if (dp) dp.value = "";
    setCategoryText("Theo danh mục");
    void loadAndRender();
    updateExportInputs(currentType, currentDate);
  });

  // First load
  await loadAndRender();
  updateExportInputs(currentType, currentDate);
});
