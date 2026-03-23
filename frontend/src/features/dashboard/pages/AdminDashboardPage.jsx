import { PageTitle } from "../../../components/PageTitle";
import { StatGrid } from "../../../components/StatGrid";

const cards = [
  { key: "users", label: "Active Users", value: "248", hint: "All roles", delta: "+8" },
  { key: "approvaltime", label: "Avg Approval Time", value: "1.8 d", hint: "Last 30 days", delta: "-0.4" },
  { key: "policy", label: "Policy Exceptions", value: "2", hint: "Need review", delta: "-1" }
];

export function AdminDashboardPage() {
  return (
    <section className="page-card">
      <PageTitle title="Admin Dashboard" subtitle="Control-center style panels for policy and system posture." />
      <StatGrid items={cards} />

      <section className="panel-grid">
        <article className="glass-panel">
          <h3>Policy Strictness</h3>
          <p>Preview slider for policy hardening intensity.</p>
          <label className="range-block">
            Strictness Level: 70%
            <input type="range" min="20" max="100" step="10" defaultValue="70" />
          </label>
          <div className="flag-row">
            <span className="flag green">MFA Enabled</span>
            <span className="flag amber">2 Pending Reviews</span>
          </div>
        </article>

        <article className="glass-panel slider-panel">
          <h3>Release Panels</h3>
          <div className="h-slider" role="region" aria-label="Release panels">
            <div className="slide-card">
              <p>Track A</p>
              <strong>Auth Hardening</strong>
              <span>Planned for Phase 3 rollout.</span>
            </div>
            <div className="slide-card">
              <p>Track B</p>
              <strong>Workflow Engine</strong>
              <span>Core approvals in Phase 5.</span>
            </div>
            <div className="slide-card">
              <p>Track C</p>
              <strong>MIS Exports</strong>
              <span>Report optimization in Phase 8.</span>
            </div>
          </div>
        </article>
      </section>
    </section>
  );
}
