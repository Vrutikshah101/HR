import { PageTitle } from "../../../components/PageTitle";
import { StatGrid } from "../../../components/StatGrid";

const cards = [
  { key: "available", label: "Available Leave", value: "10.0", hint: "Across leave types", delta: "+1.5" },
  { key: "pending", label: "Pending Requests", value: "2", hint: "Awaiting approver action", delta: "-1" },
  { key: "holidays", label: "Upcoming Holidays", value: "3", hint: "Next 30 days", delta: "+0" }
];

export function EmployeeDashboardPage() {
  return (
    <section className="page-card">
      <PageTitle
        title="Employee Dashboard"
        subtitle="Contemporary card layout with leave pulse and timeline slider preview."
      />

      <StatGrid items={cards} />

      <section className="panel-grid">
        <article className="glass-panel">
          <h3>Leave Pulse</h3>
          <p>Adjust the month focus to preview trend controls.</p>
          <label className="range-block">
            Focus Window: 60 days
            <input type="range" min="30" max="120" step="15" defaultValue="60" />
          </label>
          <div className="progress-stack">
            <div><span>Casual</span><progress value="72" max="100" /></div>
            <div><span>Sick</span><progress value="45" max="100" /></div>
            <div><span>Earned</span><progress value="28" max="100" /></div>
          </div>
        </article>

        <article className="glass-panel slider-panel">
          <h3>Upcoming Events</h3>
          <div className="h-slider" role="region" aria-label="Upcoming events">
            <div className="slide-card">
              <p>Mar 29</p>
              <strong>Quarter End Freeze</strong>
              <span>Leave approvals may take extra 1 day.</span>
            </div>
            <div className="slide-card">
              <p>Apr 04</p>
              <strong>Festival Holiday</strong>
              <span>Regional holiday for multiple departments.</span>
            </div>
            <div className="slide-card">
              <p>Apr 18</p>
              <strong>Team Offsite</strong>
              <span>Expect temporary approval rerouting.</span>
            </div>
          </div>
        </article>
      </section>
    </section>
  );
}
