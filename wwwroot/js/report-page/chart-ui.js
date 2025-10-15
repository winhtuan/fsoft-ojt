// chart-ui.js
import { labelMap, colorMap } from "./core.js";

// ===== Helpers =====
export const pad2 = (n) => String(n).padStart(2, "0");
export const toISO = (d) =>
  `${d.getFullYear()}-${pad2(d.getMonth() + 1)}-${pad2(d.getDate())}`;
export const toVN = (d) =>
  `${pad2(d.getDate())}/${pad2(d.getMonth() + 1)}/${d.getFullYear()}`;
export function parseVN(str) {
  const [dd, mm, yyyy] = (str || "").trim().split("/");
  if (dd && mm && yyyy) return `${yyyy}-${pad2(mm)}-${pad2(dd)}`;
  return null;
}

// ui helpers
export function setSpinner(visible) {
  const sp = document.getElementById("loadingSpinner");
  if (sp) sp.style.display = visible ? "block" : "none";
}
export function setChartToggles(type) {
  document
    .getElementById("barToggle")
    ?.classList.toggle("active", type === "bar");
  document
    .getElementById("pieToggle")
    ?.classList.toggle("active", type === "pie");
}
export function setCategoryText(text) {
  const el = document.getElementById("categoryText");
  if (el) el.textContent = text || "Theo danh mục";
}
export function updateExportInputs(currentType, currentDate) {
  const t = document.getElementById("exportTypeInput");
  const d = document.getElementById("exportDateInput");
  if (t) t.value = currentType;
  if (d) d.value = currentDate || "";
}

export function renderTableAndKpis(json) {
  const safeTotal = Number(json.total) || 0;
  const labels = Array.isArray(json.labels) ? json.labels : [];
  const data = Array.isArray(json.data)
    ? json.data.map((n) => Number(n) || 0)
    : [];

  const totalEl = document.getElementById("totalPlants");
  const topEl = document.getElementById("topType");
  if (totalEl) totalEl.textContent = safeTotal.toString();
  if (topEl) topEl.textContent = json.topType || "-";

  const tbody = document.querySelector("#dataTable tbody");
  if (tbody) {
    tbody.innerHTML = "";
    labels.forEach((label, i) => {
      const qty = Number(data[i]) || 0;
      const percent =
        safeTotal > 0 ? ((qty / safeTotal) * 100).toFixed(2) : "0.00";
      tbody.insertAdjacentHTML(
        "beforeend",
        `<tr><td>${label}</td><td>${qty}</td><td>${percent}%</td></tr>`
      );
    });
  }
  const totalCell = document.getElementById("tableTotal");
  const avgCell = document.getElementById("tableAverage");
  if (totalCell) totalCell.textContent = safeTotal.toString();
  if (avgCell)
    avgCell.textContent = (
      labels.length ? safeTotal / labels.length : 0
    ).toFixed(2);
}

// ===== Colors =====
function hexToRgba(hex, alpha = 1) {
  if (!hex) return `rgba(176,190,197,${alpha})`; // fallback #B0BEC5
  let h = hex.replace("#", "");
  if (h.length === 3)
    h = h
      .split("")
      .map((c) => c + c)
      .join("");
  const r = parseInt(h.slice(0, 2), 16);
  const g = parseInt(h.slice(2, 4), 16);
  const b = parseInt(h.slice(4, 6), 16);
  return `rgba(${r},${g},${b},${alpha})`;
}

function datasetLabel(type) {
  return labelMap[type] || "Số lượng cây";
}

// LẤY MÀU THEO LABELS (hỗ trợ cả kiểu array lẫn function trong colorMap)
function buildColors(type, labels) {
  const mapping = colorMap[type];
  const base = typeof mapping === "function" ? mapping(labels) : mapping || [];
  const bg = base.map((c) => hexToRgba(c, 0.85));
  const border = base.map((c) => hexToRgba(c, 1));
  return { bg, border };
}

// ===== Chart manager =====
function baseOptions(kind) {
  const isPie = kind === "pie";
  return {
    responsive: true,
    maintainAspectRatio: false,
    scales: isPie ? {} : { y: { beginAtZero: true } },
    plugins: {
      legend: { display: true, position: isPie ? "top" : "bottom" },
      tooltip: {
        callbacks: {
          label(ctx) {
            const v = ctx.parsed;
            if (isPie) {
              const total =
                ctx.chart._metasets[0].total ??
                ctx.dataset.data.reduce((a, b) => a + b, 0);
              const pct = total ? ((v / total) * 100).toFixed(2) : "0.00";
              return `${ctx.label}: ${v} (${pct}%)`;
            }
            return `${ctx.dataset.label}: ${v}`;
          },
        },
      },
    },
  };
}

export class ChartManager {
  constructor(ctx, initial = "bar") {
    this.type = initial;
    this.ctx = ctx;
    this.chart = new Chart(ctx, {
      type: this.type,
      data: {
        labels: [],
        datasets: [
          {
            label: "Số lượng cây",
            data: [],
            backgroundColor: [],
            borderColor: [],
            borderWidth: 1,
          },
        ],
      },
      options: baseOptions(this.type),
    });
    setChartToggles(this.type);
  }

  swap(to) {
    this.type = to;
    const data = this.chart.data;
    const options = baseOptions(this.type);
    this.chart.destroy();
    this.chart = new Chart(this.ctx, { type: this.type, data, options });
    setChartToggles(this.type);
  }

  updateFromJson(json, logicalType) {
    const labels = Array.isArray(json.labels) ? json.labels : [];
    const data = Array.isArray(json.data)
      ? json.data.map((n) => Number(n) || 0)
      : [];
    const { bg, border } = buildColors(logicalType, labels);

    this.chart.data.labels = labels;
    this.chart.data.datasets[0].data = data;
    this.chart.data.datasets[0].backgroundColor = bg;
    this.chart.data.datasets[0].borderColor = border;
    this.chart.data.datasets[0].label = datasetLabel(logicalType);
    this.chart.update();
  }
}
