import { useEffect, useState } from "react";
import { DataGrid } from "../../../components/DataGrid";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";
import { trackActivity } from "../../../services/activityTracker";

export function DesignationsMasterPage() {
  const [rows, setRows] = useState([]);
  const [form, setForm] = useState({ code: "", name: "" });
  const [message, setMessage] = useState("");

  async function load() {
    const res = await apiClient.get("/masters/designations");
    setRows(res.data ?? []);
  }

  useEffect(() => {
    load().catch(() => setMessage("Failed to load designations."));
  }, []);

  async function submit(event) {
    event.preventDefault();
    setMessage("");
    try {
      await apiClient.post("/masters/designations", form);
      trackActivity("MASTER_CREATE", `Created designation ${form.code}.`);
      setForm({ code: "", name: "" });
      await load();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Designation create failed.");
    }
  }

  return (
    <section className="page-card">
      <PageTitle title="Master: Designations" subtitle="Manage designation master records." />
      {message ? <p className="error-text">{message}</p> : null}
      <article className="glass-panel">
        <form className="form-grid split" onSubmit={submit}>
          <input placeholder="Designation Code" value={form.code} onChange={(e) => setForm((x) => ({ ...x, code: e.target.value }))} required />
          <input placeholder="Designation Name" value={form.name} onChange={(e) => setForm((x) => ({ ...x, name: e.target.value }))} required />
          <button type="submit">Add Designation</button>
        </form>
      </article>
      <DataGrid columns={[{ key: "code", label: "Code", sortable: true }, { key: "name", label: "Name", sortable: true }, { key: "isActive", label: "Active", sortable: true }]} rows={rows} />
    </section>
  );
}
