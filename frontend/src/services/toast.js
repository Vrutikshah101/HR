export function emitToast(message, kind = "success", durationMs = 3200) {
  if (!message) {
    return;
  }

  window.dispatchEvent(new CustomEvent("lms:toast", {
    detail: {
      id: `${Date.now()}-${Math.random().toString(16).slice(2)}`,
      message,
      kind,
      durationMs
    }
  }));
}

export function notifySuccess(message, durationMs = 3200) {
  emitToast(message, "success", durationMs);
}

export function notifyError(message, durationMs = 4200) {
  emitToast(message, "error", durationMs);
}

export function notifyInfo(message, durationMs = 2800) {
  emitToast(message, "info", durationMs);
}
