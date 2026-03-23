import { useEffect, useState } from "react";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";

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

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              {columns.map((column) => (
                <th key={column}>{column}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {rows.map((row, index) => (
              <tr key={`${index}-${Object.values(row).join("-")}`}>
                {columns.map((column) => (
                  <td key={column}>{row[column] ?? ""}</td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
