import { useEffect, useMemo, useState } from "react";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";
import { trackActivity } from "../../../services/activityTracker";

function formatExpiry(unixSeconds) {
  if (!unixSeconds) {
    return "";
  }

  const date = new Date(unixSeconds * 1000);
  return date.toLocaleString();
}

export function AnalyticsDashboardPage() {
  const [embedUrl, setEmbedUrl] = useState("");
  const [dashboardId, setDashboardId] = useState("");
  const [expiresAt, setExpiresAt] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const expiryText = useMemo(() => formatExpiry(expiresAt), [expiresAt]);

  async function loadEmbedUrl() {
    setLoading(true);
    setError("");

    try {
      const response = await apiClient.get("/reports/analytics/embed-url", {
        params: {
          dashboardId: dashboardId ? Number(dashboardId) : undefined
        }
      });

      setEmbedUrl(response.data?.embedUrl ?? "");
      setExpiresAt(response.data?.expiresAtUtcUnix ?? 0);
      trackActivity("REPORT_ANALYTICS_VIEW", "Opened analytics dashboard.", {
        dashboardId: response.data?.dashboardId
      });
    } catch (err) {
      const message = err.response?.data?.message ?? "Failed to load analytics dashboard.";
      setError(message);
      setEmbedUrl("");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadEmbedUrl();
  }, []);

  return (
    <section className="page-card">
      <PageTitle
        title="Analytics Dashboard"
        subtitle="Embedded analytics view (Metabase)."
      />

      <form
        className="form-grid split"
        onSubmit={async (event) => {
          event.preventDefault();
          await loadEmbedUrl();
        }}
      >
        <label>
          Dashboard ID (optional)
          <input
            value={dashboardId}
            onChange={(event) => setDashboardId(event.target.value.replace(/[^0-9]/g, ""))}
            placeholder="Auto by role"
          />
        </label>

        <button type="submit" disabled={loading}>
          {loading ? "Loading..." : "Load Dashboard"}
        </button>
      </form>

      {expiryText ? <p className="info-text">Secure URL expires at: {expiryText}</p> : null}
      {error ? <p className="error-text">{error}</p> : null}

      {embedUrl ? (
        <div className="analytics-embed-frame">
          <iframe
            title="Analytics Dashboard"
            src={embedUrl}
            loading="lazy"
            referrerPolicy="strict-origin-when-cross-origin"
          />
        </div>
      ) : null}
    </section>
  );
}
