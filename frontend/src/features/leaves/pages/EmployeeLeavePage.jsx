import { useEffect, useState } from "react";
import { DataGrid } from "../../../components/DataGrid";
import { PageTitle } from "../../../components/PageTitle";
import { trackActivity } from "../../../services/activityTracker";
import { apiClient } from "../../../services/apiClient";

function statusLabel(status) {
  const normalized = typeof status === "string" ? status.toLowerCase() : status;

  if (normalized === 1 || normalized === "draft") return "Draft";
  if (normalized === 2 || normalized === "pendinglevel1") return "Pending Level 1";
  if (normalized === 3 || normalized === "pendinglevel2") return "Pending Level 2";
  if (normalized === 4 || normalized === "approved") return "Approved";
  if (normalized === 5 || normalized === "rejected") return "Rejected";
  if (normalized === 6 || normalized === "cancelled") return "Cancelled";
  return String(status);
}

function canCancel(status) {
  const normalized = typeof status === "string" ? status.toLowerCase() : status;
  return normalized === 2 || normalized === 3 || normalized === 4 || normalized === "pendinglevel1" || normalized === "pendinglevel2" || normalized === "approved";
}

export function EmployeeLeavePage() {
  const [types, setTypes] = useState([]);
  const [balances, setBalances] = useState([]);
  const [leaves, setLeaves] = useState([]);
  const [message, setMessage] = useState("");
  const [form, setForm] = useState({
    leaveTypeCode: "CL",
    startDate: "",
    endDate: "",
    days: 1,
    reason: ""
  });

  async function loadData() {
    try {
      const [typesRes, balancesRes, leavesRes] = await Promise.all([
        apiClient.get("/leaves/types"),
        apiClient.get("/leaves/my-balances"),
        apiClient.get("/leaves/my")
      ]);

      setTypes(typesRes.data ?? []);
      setBalances(balancesRes.data ?? []);
      setLeaves(leavesRes.data ?? []);
    } catch {
      setMessage("Failed to load leave data.");
    }
  }

  useEffect(() => {
    loadData();
  }, []);

  async function handleSubmit(event) {
    event.preventDefault();
    setMessage("");

    try {
      await apiClient.post("/leaves", {
        leaveTypeCode: form.leaveTypeCode,
        startDate: form.startDate,
        endDate: form.endDate,
        days: Number(form.days),
        reason: form.reason
      });

      setMessage("Leave request submitted.");
      trackActivity("LEAVE_APPLY", `Leave submitted for ${form.startDate} to ${form.endDate}.`);
      setForm((old) => ({ ...old, reason: "" }));
      await loadData();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Submit failed.");
    }
  }

  async function handleCancel(id) {
    try {
      await apiClient.post(`/leaves/${id}/cancel`);
      setMessage("Leave cancelled.");
      trackActivity("LEAVE_CANCEL", `Cancelled leave request ${id}.`);
      await loadData();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Cancel failed.");
    }
  }

  return (
    <section className="page-card">
      <PageTitle title="My Leaves" subtitle="Apply, track history, and manage leave balance." />

      {message ? <p className="info-text">{message}</p> : null}

      <section className="panel-grid">
        <article className="glass-panel">
          <h3>Apply Leave</h3>
          <form className="form-grid split" onSubmit={handleSubmit}>
            <label>
              Leave Type
              <select value={form.leaveTypeCode} onChange={(e) => setForm((x) => ({ ...x, leaveTypeCode: e.target.value }))}>
                {(types.length ? types : [{ code: "CL", name: "Casual Leave" }]).map((type) => (
                  <option key={type.code} value={type.code}>{type.code} - {type.name}</option>
                ))}
              </select>
            </label>

            <label>
              Start Date
              <input type="date" value={form.startDate} onChange={(e) => setForm((x) => ({ ...x, startDate: e.target.value }))} required />
            </label>

            <label>
              End Date
              <input type="date" value={form.endDate} onChange={(e) => setForm((x) => ({ ...x, endDate: e.target.value }))} required />
            </label>

            <label>
              Days
              <input type="number" min="0.5" step="0.5" value={form.days} onChange={(e) => setForm((x) => ({ ...x, days: e.target.value }))} required />
            </label>

            <label className="full-width">
              Reason
              <textarea rows="3" value={form.reason} onChange={(e) => setForm((x) => ({ ...x, reason: e.target.value }))} required />
            </label>

            <button type="submit">Submit Leave Request</button>
          </form>
        </article>

        <article className="glass-panel">
          <h3>Current Balances</h3>
          <DataGrid
            rows={balances ?? []}
            searchPlaceholder="Search leave balances..."
            columns={[
              { key: "leaveTypeCode", label: "Type", sortable: true, filterable: true },
              { key: "available", label: "Available", sortable: true }
            ]}
          />
        </article>
      </section>

      <DataGrid
        rows={leaves ?? []}
        searchPlaceholder="Search leave requests..."
        columns={[
          { key: "id", label: "Request", sortable: true },
          { key: "leaveTypeCode", label: "Type", sortable: true, filterable: true },
          { key: "startDate", label: "Dates", sortable: true, render: (row) => `${row.startDate} to ${row.endDate}` },
          { key: "days", label: "Days", sortable: true },
          { key: "status", label: "Status", sortable: true, filterable: true, render: (row) => statusLabel(row.status) },
          {
            key: "action",
            label: "Action",
            sortable: false,
            render: (row) => canCancel(row.status) ? (
              <button type="button" className="secondary" onClick={() => handleCancel(row.id)}>
                Cancel
              </button>
            ) : "-"
          }
        ]}
      />
    </section>
  );
}
