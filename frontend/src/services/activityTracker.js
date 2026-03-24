import { emitToast } from "./toast";

const storageKey = "lms.activity.v1";
const maxItems = 200;
let memoryFallback = [];

function emitUpdated() {
  window.dispatchEvent(new CustomEvent("lms:activity-updated"));
}

function readAll() {
  try {
    return JSON.parse(localStorage.getItem(storageKey) ?? "[]");
  } catch {
    return memoryFallback;
  }
}

function writeAll(items) {
  const trimmed = items.slice(0, maxItems);
  memoryFallback = trimmed;

  try {
    localStorage.setItem(storageKey, JSON.stringify(trimmed));
  } catch {
    // Keep in-memory activity list when storage is blocked/unavailable.
  }

  emitUpdated();
}

export function trackActivity(type, message, metadata = {}) {
  const next = {
    id: `${Date.now()}-${Math.random().toString(16).slice(2)}`,
    type,
    message,
    metadata,
    at: new Date().toISOString()
  };

  const items = readAll();
  items.unshift(next);
  writeAll(items);

  if (type !== "PAGE_VISIT") {
    emitToast(message, "success", 2500);
  }
}

export function trackPageVisit(pathname) {
  trackActivity("PAGE_VISIT", `Visited ${pathname}`, { pathname });
}

export function getRecentActivities(limit = 20) {
  return readAll().slice(0, limit);
}
