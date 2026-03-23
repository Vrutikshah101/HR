import { PageTitle } from "../../../components/PageTitle";
import { StatGrid } from "../../../components/StatGrid";

const cards = [
  { key: "open", label: "Open Requests", value: "14", hint: "Across departments", delta: "+2" },
  { key: "pending-l2", label: "Pending Level 2", value: "5", hint: "Needs senior approver", delta: "-1" },
  { key: "onleave", label: "Employees On Leave", value: "9", hint: "Current day", delta: "+3" }
];

export function HrDashboardPage() {
  return (
    <section className="page-card">
      <PageTitle title="HR Dashboard" subtitle="Operations-focused workspace with workload sliders and panel cards." />
      <StatGrid items={cards} />

      <section className="panel-grid">
        <article className="glass-panel">
          <h3>Approval Capacity Planner</h3>
          <p>Static slider to preview weekly approval load balancing controls.</p>
          <label className="range-block">
            Max requests per approver: 18
            <input type="range" min="6" max="30" defaultValue="18" />
          </label>
          <ul className="compact-list">
            <li>Engineering queue: High</li>
            <li>Support queue: Medium</li>
            <li>Operations queue: Low</li>
          </ul>
        </article>

        <article className="glass-panel">
          <h3>Department Heat Panels</h3>
          <div className="heat-grid">
            <div className="heat-card hot"><span>Engineering</span><strong>34%</strong></div>
            <div className="heat-card warm"><span>HR</span><strong>19%</strong></div>
            <div className="heat-card cool"><span>Finance</span><strong>11%</strong></div>
          </div>
        </article>
      </section>
    </section>
  );
}
