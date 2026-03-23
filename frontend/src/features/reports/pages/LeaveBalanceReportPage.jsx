import { PageTitle } from "../../../components/PageTitle";

export function LeaveBalanceReportPage() {
  return (
    <section className="page-card">
      <PageTitle
        title="Leave Balance Report"
        subtitle="Report-centric layout with modern filter panel and utilization slider."
      />

      <section className="panel-grid">
        <article className="glass-panel">
          <h3>Filters</h3>
          <form className="form-grid split" onSubmit={(event) => event.preventDefault()}>
            <label>
              Department
              <select defaultValue="all">
                <option value="all">All Departments</option>
                <option value="engineering">Engineering</option>
                <option value="hr">HR</option>
              </select>
            </label>

            <label>
              Leave Type
              <select defaultValue="all">
                <option value="all">All Leave Types</option>
                <option value="cl">Casual Leave</option>
                <option value="sl">Sick Leave</option>
              </select>
            </label>

            <label>
              As On Date
              <input type="date" />
            </label>

            <button type="submit">Apply Filters</button>
          </form>
        </article>

        <article className="glass-panel">
          <h3>Utilization Threshold</h3>
          <p>Set the highlight threshold for high leave usage rows.</p>
          <label className="range-block">
            Threshold: 70%
            <input type="range" min="40" max="95" step="5" defaultValue="70" />
          </label>
          <div className="action-row">
            <button type="button">Export CSV</button>
            <button type="button" className="secondary">Export XLSX</button>
          </div>
        </article>
      </section>

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Employee Code</th>
              <th>Name</th>
              <th>Department</th>
              <th>Leave Type</th>
              <th>Available</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>E001</td>
              <td>Asha Sharma</td>
              <td>Engineering</td>
              <td>CL</td>
              <td>7.5</td>
            </tr>
            <tr>
              <td>E014</td>
              <td>Rohit Das</td>
              <td>HR</td>
              <td>SL</td>
              <td>5.0</td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>
  );
}
