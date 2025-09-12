import { TaskStatusCode } from "../constants/status.js";

export const BASE_URL =
  (typeof process !== "undefined" && process.env && process.env.VITE_API_URL) ||
  (typeof globalThis !== "undefined" && (globalThis.__VITE_API_URL__ || "")) ||
  "";

export async function api(path, init) {
    const res = await fetch(`${BASE_URL}${path}`, {
        headers: { "Content-Type": "application/json" },
        ...(init || {}),
    });
    if (!res.ok) {
        let msg = `HTTP ${res.status}`;
        try { const j = await res.json(); msg = j.message || msg; } catch { }
        throw new Error(msg);
    }
    if (res.status === 204) return undefined;
    return await res.json();
}


export function fetchBoards() {
    return api(`/api/v1/boards?includeStats=false`);
}


export function fetchTasksSorted(boardId, userId, status) {
    return api(`/api/v1/boards/${boardId}/tasks/sorted?userId=${userId}&status=${status}`);
}


export function fetchTask(boardId, taskId, userId) {
    const qs = userId ? `?userId=${userId}` : "";
    return api(`/api/v1/boards/${boardId}/tasks/${taskId}${qs}`);
}


export function createTask(boardId, input) {
    return api(`/api/v1/boards/${boardId}/tasks`, { method: "POST", body: JSON.stringify(input) });
}


export function updateTaskStatus(boardId, taskId, status) {
    return api(`/api/v1/boards/${boardId}/tasks/${taskId}/status`, { method: "PATCH", body: JSON.stringify({ status: TaskStatusCode[status] }) });
}

export function fetchTasksByStatuses(boardId, userId, statuses) {
    const qs = new URLSearchParams();
    if (userId) qs.set("userId", userId);
    (statuses || []).forEach((s) => qs.append("statuses", s));
    return api(`/api/v1/boards/${boardId}/tasks/by-status?${qs.toString()}`);
}
