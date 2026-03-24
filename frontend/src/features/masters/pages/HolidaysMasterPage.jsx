import { useEffect, useState } from "react";
import { DataGrid } from "../../../components/DataGrid";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";
import { trackActivity } from "../../../services/activityTracker";

export function HolidaysMasterPage() {
  const [rows, setRows] = useState([]);
  const [form, setForm] = useState({ name: "", date: "", location: "", isOptional: false });
  const [message, setMessage] = useState("");

  async function load() {
    const res = await apiClient.get("/masters/holidays");
    setRows(res.data ?? []);
  }

  useEffect(() => {
    load().catch(() => setMessage("Failed to load holidays."));
  }, []);

  async function submit(event) {
    event.preventDefault();
    setMessage("");
    try {
      await apiClient.post("/masters/holidays", form);
      trackActivity("MASTER_CREATE", `Created holiday ${form.name}.`);
      setForm({ name: "", date: "", location: "", isOptional: false });
      await load();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Holiday create failed.");
    }
  }

  return (
    <section className="page-card">
      <PageTitle title="Master: Holidays" subtitle="Manage holiday master records." />
      {message ? <p className="error-text">{message}</p> : null}
      <article className="glass-panel">
        <form className="form-grid split" onSubmit={submit}>
          <input placeholder="Holiday Name" value={form.name} onChange={(e) => setForm((x) => ({ ...x, name: e.target.value }))} required />
          <input type="date" value={form.date} onChange={(e) => setForm((x) => ({ ...x, date: e.target.value }))} required />
          <input placeholder="Location (optional)" value={form.location} onChange={(e) => setForm((x) => ({ ...x, location: e.target.value }))} />
          <label><input type="checkbox" checked={form.isOptional} onChange={(e) => setForm((x) => ({ ...x, isOptional: e.target.checked }))} /> Optional holiday</label>
          <button type="submit">Add Holiday</button>
        </form>
      </article>
      <DataGrid columns={[{ key: "name", label: "Name", sortable: true }, { key: "date", label: "Date", sortable: true }, { key: "location", label: "Location", sortable: true }, { key: "isOptional", label: "Optional", sortable: true }]} rows={rows} />
    </section>
  );
}
