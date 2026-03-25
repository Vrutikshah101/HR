import { useEffect, useState } from "react";
import { DataGrid } from "../../../components/DataGrid";
import { PageTitle } from "../../../components/PageTitle";
import { trackActivity } from "../../../services/activityTracker";
import { apiClient } from "../../../services/apiClient";
import { ReportCharts } from "../../../components/charts/ReportCharts";

export function LeaveBalanceReportPage() {
  const [department, setDepartment] = useState("");
  const [leaveTypeCode, setLeaveTypeCode] = useState("");
  const [rows, setRows] = useState([]);
  const [message, setMessage] = useState("");

  async function load(filters = {}) {
    try {
      const response = await apiClient.get("/reports/leave-balance", { params: filters });
      const raw = response.data ?? [];
      const mapped = raw.map((x) => x.fields ?? {});
      setRows(mapped);
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Failed to load report.");
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function applyFilters(event) {
    event.preventDefault();
    setMessage("");
    trackActivity("REPORT_FILTER", "Applied Leave Balance report filters.", { department, leaveTypeCode });
    await load({
      department: department || undefined,
      leaveTypeCode: leaveTypeCode || undefined
    });
  }

  async function exportCsv() {
    try {
      const response = await apiClient.get("/reports/leave-balance", {
        params: {
          department: department || undefined,
          leaveTypeCode: leaveTypeCode || undefined,
          format: "csv"
        },
        responseType: "blob"
      });

      const url = URL.createObjectURL(response.data);
      const a = document.createElement("a");
      a.href = url;
      a.download = "leave-balance.csv";
      a.click();
      URL.revokeObjectURL(url);
      trackActivity("REPORT_EXPORT", "Exported Leave Balance CSV.");
    } catch {
      setMessage("CSV export failed.");
    }
  }

  const columns = rows.length ? Object.keys(rows[0]) : [];

  return (
    <section className="page-card">
      <PageTitle title="Leave Balance Report" subtitle="Filter and export live report data." />
      {message ? <p className="info-text">{message}</p> : null}

      <form className="form-grid split" onSubmit={applyFilters}>
        <label>
          Department
          <input value={department} onChange={(e) => setDepartment(e.target.value)} placeholder="e.g. Engineering" />
        </label>

        <label>
          Leave Type
          <input value={leaveTypeCode} onChange={(e) => setLeaveTypeCode(e.target.value)} placeholder="e.g. CL" />
        </label>

        <button type="submit">Apply Filters</button>
      </form>

      <div className="action-row">
        <button type="button" onClick={exportCsv}>Export CSV</button>
      </div>

      <ReportCharts rows={rows} />

      <DataGrid
        rows={rows}
        searchPlaceholder="Search report rows..."
        columns={columns.map((column) => ({
          key: column,
          label: column,
          sortable: true,
          filterable: true
        }))}
      />
    </section>
  );
}
