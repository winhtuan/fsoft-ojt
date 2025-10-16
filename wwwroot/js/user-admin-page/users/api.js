// users/api.js
import { apiFetch } from "../core/api-client.js";

export const userApi = {
  list: (q = "", status = "", page = 1, pageSize = 10) =>
    apiFetch(
      `/api/admin/users/list?q=${encodeURIComponent(
        q
      )}&status=${status}&page=${page}&pageSize=${pageSize}`
    ),

  getById: (id) => apiFetch(`/api/admin/users/${id}`),

  history: (id, take = 10) =>
    apiFetch(`/api/admin/users/${id}/history?take=${take}`),

  create: (data) =>
    apiFetch(`/api/admin/users/create`, {
      method: "POST",
      body: JSON.stringify(data),
    }),

  update: (data) =>
    apiFetch(`/api/admin/users/update`, {
      method: "POST",
      body: JSON.stringify(data),
    }),

  delete: (id) =>
    apiFetch(`/api/admin/users/delete`, {
      method: "POST",
      body: JSON.stringify({ id }),
    }),

  restore: (id) =>
    apiFetch(`/api/admin/users/restore`, {
      method: "POST",
      body: JSON.stringify({ id }),
    }),
};
