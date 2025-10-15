// ===== Labels & Colors =====
export const labelMap = {
  plantType: "Số lượng theo Loại cây",
  region: "Số lượng theo Vùng miền",
  climate: "Số lượng theo Vùng khí hậu",
  soil: "Số lượng theo Loại đất",
  usage: "Số lượng theo Mục đích",
};
export const plantTypeColorByLabel = {
  "Cây thân gỗ": "#8D6E63",
  "Cây ăn quả": "#F4A261",
  "Cây công nghiệp": "#2A9D8F",
  "Cây lương thực": "#DDB945",
  "Cây cảnh": "#9FA8DA",
  "Cây rau màu & gia vị": "#43A047",
};
export const regionColorByLabel = {
  "Đồng bằng sông Hồng": "#1B4965",
  "Đồng bằng sông Cửu Long": "#2C7DA0",
  "Duyên hải Nam Trung Bộ": "#468FAF",
  "Tây Nguyên": "#61A5C2",
  "Trung du và miền núi Bắc Bộ": "#89C2D9",
  "Đông Nam Bộ": "#A9D6E5",
};
export const climateColorByLabel = {
  "Nhiệt đới gió mùa": "#E63946",
  "Khí hậu ven biển": "#EF476F",
  "Ôn đới núi cao": "#F77F00",
  "Khí hậu khô hạn": "#E09F3E",
  "Khí hậu mát mẻ": "#F4A261",
  "Cận xích đạo": "#FFD166",
};
export const soilColorByLabel = {
  "Đất phù sa": "#4A342E",
  "Đất đồi núi": "#6D4C41",
  "Đất cát ven biển": "#8D6E63",
  "Đất đỏ bazan": "#9C6B5E",
  "Đất mặn": "#BCAAA4",
  "Đất xám": "#D7CCC8",
};
export const usageColorByLabel = {
  "Cảnh quan": "#2D6A4F",
  "Thực phẩm": "#1B5E20",
  "Nguyên liệu công nghiệp": "#8D6E63",
  "Dược liệu": "#A7C957",
  "Thức ăn chăn nuôi": "#52B788",
  Khác: "#40916C",
};
const mapColors = (labels, dict, fallback = "#B0BEC5") =>
  labels.map((l) => dict[l] ?? fallback);

export const colorMap = {
  plantType: (labels) => mapColors(labels, plantTypeColorByLabel),
  region: (labels) => mapColors(labels, regionColorByLabel),
  climate: (labels) => mapColors(labels, climateColorByLabel),
  soil: (labels) => mapColors(labels, soilColorByLabel),
  usage: (labels) => mapColors(labels, usageColorByLabel),
};

// ===== Helpers =====
export const pad2 = (n) => String(n).padStart(2, "0");

export function toISO(d) {
  return `${d.getFullYear()}-${pad2(d.getMonth() + 1)}-${pad2(d.getDate())}`;
}
export function toVN(d) {
  return `${pad2(d.getDate())}/${pad2(d.getMonth() + 1)}/${d.getFullYear()}`;
}
export function parseVN(str) {
  const [dd, mm, yyyy] = (str || "").trim().split("/");
  if (dd && mm && yyyy) return `${yyyy}-${pad2(mm)}-${pad2(dd)}`;
  return null;
}
