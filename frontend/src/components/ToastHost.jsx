import { useEffect, useState } from "react";

export function ToastHost() {
  const [toasts, setToasts] = useState([]);

  useEffect(() => {
    function onToast(event) {
      const toast = event.detail;
      if (!toast?.message) {
        return;
      }

      setToasts((prev) => [...prev, toast]);

      window.setTimeout(() => {
        setToasts((prev) => prev.filter((x) => x.id !== toast.id));
      }, toast.durationMs ?? 3200);
    }

    window.addEventListener("lms:toast", onToast);
    return () => window.removeEventListener("lms:toast", onToast);
  }, []);

  return (
    <div className="toast-stack" aria-live="polite" aria-atomic="true">
      {toasts.map((toast) => (
        <div key={toast.id} className={`toast-item ${toast.kind ?? "success"}`}>
          {toast.message}
        </div>
      ))}
    </div>
  );
}
