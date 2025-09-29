document.addEventListener("DOMContentLoaded", function () {
  let currentType = "plantType";
  let currentDate = null;

  const labelMap = {
    plantType: "Số lượng theo Loại cây",
    region: "Số lượng theo Vùng miền",
    climate: "Số lượng theo Vùng khí hậu",
    soil: "Số lượng theo Loại đất",
    usage: "Số lượng theo Mục đích",
  };
  const colorMap = {
    plantType: [
      // Tông màu Hồng/Đỏ
      "rgba(255, 99, 132, 0.7)",
      "rgba(255, 159, 64, 0.7)",
      "rgba(255, 205, 86, 0.7)",
    ],
    region: [
      // Tông màu Xanh dương/Xanh lá
      "rgba(54, 162, 235, 0.7)",
      "rgba(75, 192, 192, 0.7)",
      "rgba(152, 251, 152, 0.7)",
    ],
    climate: [
      // Tông màu Cam/Vàng
      "rgba(255, 165, 0, 0.7)",
      "rgba(255, 215, 0, 0.7)",
      "rgba(244, 164, 96, 0.7)",
    ],
    soil: [
      // Tông màu Nâu/Xám
      "rgba(139, 69, 19, 0.7)",
      "rgba(160, 82, 45, 0.7)",
      "rgba(128, 128, 128, 0.7)",
    ],
    usage: [
      // Tông màu Tím/Xanh mòng két
      "rgba(153, 102, 255, 0.7)",
      "rgba(0, 128, 128, 0.7)",
      "rgba(72, 61, 139, 0.7)",
    ],
  };
  const colors = [
    "rgba(255, 99, 132, 0.7)",
    "rgba(54, 162, 235, 0.7)",
    "rgba(255, 206, 86, 0.7)",
    "rgba(75, 192, 192, 0.7)",
    "rgba(153, 102, 255, 0.7)",
    "rgba(255, 159, 64, 0.7)",
    "rgba(199, 199, 199, 0.7)",
    "rgba(83, 102, 255, 0.7)",
    "rgba(255, 99, 255, 0.7)",
  ];
  const borderColors = colors.map((c) => c.replace("0.7", "1"));

  const ctx = document.getElementById("barChart").getContext("2d");
  const barChart = new Chart(ctx, {
    type: "bar",
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
    options: { responsive: true, scales: { y: { beginAtZero: true } } },
  });

  const datePickerEl = document.getElementById("datePicker");
  const td = new tempusDominus.TempusDominus(datePickerEl, {
    display: {
      components: {
        decades: false,
        year: true,
        month: true,
        date: true,
        hours: false,
        minutes: false,
        seconds: false,
      },
      buttons: { today: false, clear: true, close: true },
      placement: "bottom",
    },
    localization: { format: "dd/MM/yyyy", locale: "vi" },
  });

  function updateExportInputs() {
    document.getElementById("exportTypeInput").value = currentType;
    document.getElementById("exportDateInput").value = currentDate || "";
  }

  async function loadChartData(type = currentType, date = currentDate) {
    let url = `?handler=ChartData&type=${type}`;
    if (date) url += `&date=${date}`;

    try {
      const res = await fetch(url);
      if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);

      const json = await res.json();

      const currentColors = colorMap[type] || colorMap.plantType;
      const currentBorderColors = currentColors.map((c) =>
        c.replace("0.7", "1")
      );

      barChart.data.labels = json.labels;
      barChart.data.datasets[0].data = json.data;
      barChart.data.datasets[0].backgroundColor = json.labels.map(
        (_, i) => currentColors[i % currentColors.length]
      );
      barChart.data.datasets[0].borderColor = json.labels.map(
        (_, i) => currentBorderColors[i % currentBorderColors.length]
      );
      barChart.data.datasets[0].label = labelMap[type] || "Số lượng cây";
      barChart.update();
    } catch (error) {
      console.error("Failed to load chart data:", error);
    }
  }

  function pad2(n) {
    return String(n).padStart(2, "0");
  }
  function toISO(d) {
    return `${d.getFullYear()}-${pad2(d.getMonth() + 1)}-${pad2(d.getDate())}`;
  }
  function toVN(d) {
    return `${pad2(d.getDate())}/${pad2(d.getMonth() + 1)}/${d.getFullYear()}`;
  }
  datePickerEl.addEventListener("change.td", function (e) {
    const d = e.detail?.date;
    if (d instanceof Date && !isNaN(d)) {
      const isoDate = toISO(d); // yyyy-MM-dd
      const vnDate = toVN(d); // dd/MM/yyyy
      datePickerEl.value = vnDate;
      currentDate = isoDate;
    } else {
      datePickerEl.value = "";
      currentDate = null;
    }
    loadChartData(currentType, currentDate);
    updateExportInputs();
  });

  $(".category-option").on("click", function (e) {
    e.preventDefault();
    currentType = $(this).data("category");
    $("#categoryDropdown").text($(this).text());
    loadChartData(currentType, currentDate);
    updateExportInputs();
  });

  const initialCategoryText =
    $(`.category-option[data-category='${currentType}']`).text() ||
    "Theo loại cây";
  $("#categoryDropdown").text(initialCategoryText);

  loadChartData(currentType, currentDate);
  updateExportInputs();
});
