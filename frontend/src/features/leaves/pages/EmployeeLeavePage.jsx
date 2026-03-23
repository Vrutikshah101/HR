import { PageTitle } from "../../../components/PageTitle";

export function EmployeeLeavePage() {
  return (
    <section className="page-card">
      <PageTitle title="My Leaves" subtitle="Panel-driven apply flow with live-style slider controls and request board." />

      <section className="panel-grid">
        <article className="glass-panel">
          <h3>Apply Leave</h3>
          <form className="form-grid split" onSubmit={(event) => event.preventDefault()}>
            <label>
              Leave Type
              <select defaultValue="CL">
                <option value="CL">Casual Leave (CL)</option>
                <option value="SL">Sick Leave (SL)</option>
                <option value="EL">Earned Leave (EL)</option>
              </select>
            </label>

            <label>
              Start Date
              <input type="date" required />
            </label>

            <label>
              End Date
              <input type="date" required />
            </label>

            <label>
              Days
              <input type="number" min="0.5" step="0.5" defaultValue="1" required />
            </label>

            <label className="full-width">
              Reason
              <textarea rows="3" placeholder="Enter reason for leave" />
            </label>

            <button type="submit">Submit Leave Request</button>
          </form>
        </article>

        <article className="glass-panel">
          <h3>Plan Leave Window</h3>
          <p>Static slider to visualize planning range before applying.</p>
          <label className="range-block">
            Planning horizon: 45 days
            <input type="range" min="15" max="120" step="15" defaultValue="45" />
          </label>

          <div className="progress-stack">
            <div><span>CL Balance</span><progress value="75" max="100" /></div>
            <div><span>SL Balance</span><progress value="50" max="100" /></div>
            <div><span>EL Balance</span><progress value="35" max="100" /></div>
          </div>
        </article>
      </section>

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Request Id</th>
              <th>Type</th>
              <th>Dates</th>
              <th>Status</th>
              <th>Stage</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>LV-1001</td>
              <td>CL</td>
              <td>2026-03-26 to 2026-03-27</td>
              <td><span className="badge pending">Pending</span></td>
              <td>Level 1</td>
            </tr>
            <tr>
              <td>LV-0992</td>
              <td>SL</td>
              <td>2026-03-04 to 2026-03-04</td>
              <td><span className="badge approved">Approved</span></td>
              <td>Completed</td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>
  );
}
