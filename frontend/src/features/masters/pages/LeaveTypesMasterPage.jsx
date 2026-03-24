import { useEffect, useState } from "react";
import { DataGrid } from "../../../components/DataGrid";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";
import { trackActivity } from "../../../services/activityTracker";

export function LeaveTypesMasterPage() {
  const [rows, setRows] = useState([]);
  const [form, setForm] = useState({ code: "", name: "", requiresAttachment: false, isPaid: true, isHalfDayAllowed: true, isBackdatedAllowed: false, maxDaysPerRequest: "" });
  const [message, setMessage] = useState("");

  async function load() {
    const res = await apiClient.get("/masters/leave-types");
    setRows(res.data ?? []);
  }

  useEffect(() => {
    load().catch(() => setMessage("Failed to load leave types."));
  }, []);

  async function submit(event) {
    event.preventDefault();
    setMessage("");
    try {
      await apiClient.post("/masters/leave-types", {
        ...form,
        maxDaysPerRequest: form.maxDaysPerRequest ? Number(form.maxDaysPerRequest) : null
      });
      trackActivity("MASTER_CREATE", `Created leave type ${form.code}.`);
      setForm({ code: "", name: "", requiresAttachment: false, isPaid: true, isHalfDayAllowed: true, isBackdatedAllowed: false, maxDaysPerRequest: "" });
      await load();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Leave type create failed.");
    }
  }

  return (
    <section className="page-card">
      <PageTitle title="Master: Leave Types" subtitle="Manage leave type configuration." />
      {message ? <p className="error-text">{message}</p> : null}
      <article className="glass-panel">
        <form className="form-grid split" onSubmit={submit}>
          <input placeholder="Leave Code" value={form.code} onChange={(e) => setForm((x) => ({ ...x, code: e.target.value }))} required />
          <input placeholder="Leave Name" value={form.name} onChange={(e) => setForm((x) => ({ ...x, name: e.target.value }))} required />
          <input type="number" step="0.5" min="0" placeholder="Max days/request (optional)" value={form.maxDaysPerRequest} onChange={(e) => setForm((x) => ({ ...x, maxDaysPerRequest: e.target.value }))} />
          <label><input type="checkbox" checked={form.requiresAttachment} onChange={(e) => setForm((x) => ({ ...x, requiresAttachment: e.target.checked }))} /> Requires attachment</label>
          <label><input type="checkbox" checked={form.isPaid} onChange={(e) => setForm((x) => ({ ...x, isPaid: e.target.checked }))} /> Paid leave</label>
          <label><input type="checkbox" checked={form.isHalfDayAllowed} onChange={(e) => setForm((x) => ({ ...x, isHalfDayAllowed: e.target.checked }))} /> Half-day allowed</label>
          <label><input type="checkbox" checked={form.isBackdatedAllowed} onChange={(e) => setForm((x) => ({ ...x, isBackdatedAllowed: e.target.checked }))} /> Backdated allowed</label>
          <button type="submit">Add Leave Type</button>
        </form>
      </article>
      <DataGrid columns={[{ key: "code", label: "Code", sortable: true }, { key: "name", label: "Name", sortable: true }, { key: "isPaid", label: "Paid", sortable: true }, { key: "isActive", label: "Active", sortable: true }]} rows={rows} />
    </section>
  );
}
