export async function fetchChartData(type, date) {
  let url = `/api/report/chart-data?type=${encodeURIComponent(type)}`;
  if (date) url += `&date=${encodeURIComponent(date)}`;

  const res = await fetch(url, {
    headers: { "X-Requested-With": "XMLHttpRequest" },
  });
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  return res.json();
}
