import { PageTitle } from "../../../components/PageTitle";

export function ApprovalsPage() {
  return (
    <section className="page-card">
      <PageTitle
        title="Team Approvals"
        subtitle="Split-panel approval board with level filters and action cards."
      />

      <section className="panel-grid">
        <article className="glass-panel">
          <h3>Queue Filter</h3>
          <label className="range-block">
            Prioritize urgency: 65%
            <input type="range" min="0" max="100" defaultValue="65" />
          </label>
          <div className="flag-row">
            <span className="flag amber">5 Level-2 Pending</span>
            <span className="flag green">9 Within SLA</span>
          </div>
        </article>

        <article className="glass-panel slider-panel">
          <h3>Approval Lanes</h3>
          <div className="h-slider" role="region" aria-label="Approval lanes">
            <div className="slide-card">
              <p>Lane 1</p>
              <strong>Engineering - L1</strong>
              <span>3 requests awaiting manager review.</span>
            </div>
            <div className="slide-card">
              <p>Lane 2</p>
              <strong>Support - L2</strong>
              <span>2 requests escalated to L2 approver.</span>
            </div>
            <div className="slide-card">
              <p>Lane 3</p>
              <strong>HR - L1</strong>
              <span>4 requests planned for same-day closure.</span>
            </div>
          </div>
        </article>
      </section>

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Request Id</th>
              <th>Employee</th>
              <th>Dates</th>
              <th>Current Level</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>LV-1001</td>
              <td>E001 / Asha Sharma</td>
              <td>2026-03-26 to 2026-03-27</td>
              <td>Level 1</td>
              <td>
                <div className="action-row">
                  <button type="button">Approve</button>
                  <button type="button" className="secondary">Reject</button>
                </div>
              </td>
            </tr>
            <tr>
              <td>LV-0998</td>
              <td>E014 / Rohit Das</td>
              <td>2026-03-30 to 2026-04-01</td>
              <td>Level 2</td>
              <td>
                <div className="action-row">
                  <button type="button">Approve</button>
                  <button type="button" className="secondary">Reject</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>
  );
}
