// core/api-client.js
export async function apiFetch(url, options = {}) {
  const defaultHeaders = {
    Accept: "application/json",
    "Content-Type": "application/json",
  };

  const opts = {
    credentials: "include",
    ...options,
    headers: { ...defaultHeaders, ...(options.headers || {}) },
  };

  const res = await fetch(url, opts);
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`HTTP ${res.status}: ${text}`);
  }

  try {
    return await res.json();
  } catch {
    return null;
  }
}
