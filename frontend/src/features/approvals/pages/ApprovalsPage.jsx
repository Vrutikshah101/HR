import { useEffect, useState } from "react";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";

function statusLabel(status) {
  const normalized = typeof status === "string" ? status.toLowerCase() : status;
  if (normalized === 2 || normalized === "pendinglevel1") return "Pending Level 1";
  if (normalized === 3 || normalized === "pendinglevel2") return "Pending Level 2";
  if (normalized === 4 || normalized === "approved") return "Approved";
  if (normalized === 5 || normalized === "rejected") return "Rejected";
  if (normalized === 6 || normalized === "cancelled") return "Cancelled";
  return String(status);
}

export function ApprovalsPage() {
  const [rows, setRows] = useState([]);
  const [message, setMessage] = useState("");
  const [comments, setComments] = useState({});

  async function loadPending() {
    try {
      const response = await apiClient.get("/leaves/team-pending");
      setRows(response.data ?? []);
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Failed to load approvals.");
    }
  }

  useEffect(() => {
    loadPending();
  }, []);

  async function runAction(id, action) {
    try {
      await apiClient.post(`/leaves/${id}/${action}`, { comment: comments[id] ?? "" });
      setMessage(`Request ${action}d.`);
      await loadPending();
    } catch (err) {
      setMessage(err.response?.data?.message ?? `${action} failed.`);
    }
  }

  return (
    <section className="page-card">
      <PageTitle title="Team Approvals" subtitle="Approve or reject pending requests." />
      {message ? <p className="info-text">{message}</p> : null}

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Request Id</th>
              <th>Employee Id</th>
              <th>Dates</th>
              <th>Status</th>
              <th>Comment</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {(rows ?? []).map((row) => (
              <tr key={row.id}>
                <td>{row.id}</td>
                <td>{row.employeeId}</td>
                <td>{row.startDate} to {row.endDate}</td>
                <td>{statusLabel(row.status)}</td>
                <td>
                  <input
                    value={comments[row.id] ?? ""}
                    onChange={(e) => setComments((x) => ({ ...x, [row.id]: e.target.value }))}
                    placeholder="Comment (required for reject)"
                  />
                </td>
                <td>
                  <div className="action-row">
                    <button type="button" onClick={() => runAction(row.id, "approve")}>Approve</button>
                    <button
                      type="button"
                      className="secondary"
                      disabled={!(comments[row.id] ?? "").trim()}
                      onClick={() => runAction(row.id, "reject")}
                    >
                      Reject
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
