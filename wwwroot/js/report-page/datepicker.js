import { toISO, toVN, parseVN } from "./core.js";

export function setupDatePicker(inputEl, { onApply }) {
  const td = new tempusDominus.TempusDominus(inputEl, {
    display: {
      theme: "light",
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

  let currentDate = null;
  let pendingDate = null;

  inputEl.addEventListener("change.td", (e) => {
    const d = e.detail?.date;
    if (d instanceof Date && !isNaN(d)) {
      pendingDate = toISO(d);
      inputEl.value = toVN(d);
    } else {
      pendingDate = null;
      inputEl.value = "";
    }
  });

  inputEl.addEventListener("hide.td", () => {
    if (pendingDate !== currentDate) {
      currentDate = pendingDate;
      onApply?.(currentDate);
    }
  });

  inputEl.addEventListener("keydown", (ev) => {
    if (ev.key === "Enter") {
      ev.preventDefault();
      pendingDate = parseVN(inputEl.value) || null;
      td.hide(); // sẽ kích hoạt hide.td
    }
  });

  return {
    getDate: () => currentDate,
    clear: () => {
      pendingDate = null;
      currentDate = null;
      inputEl.value = "";
      onApply?.(currentDate);
    },
  };
}
