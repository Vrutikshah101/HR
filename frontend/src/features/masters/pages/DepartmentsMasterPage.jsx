import { useEffect, useState } from "react";
import { DataGrid } from "../../../components/DataGrid";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";
import { trackActivity } from "../../../services/activityTracker";

export function DepartmentsMasterPage() {
  const [rows, setRows] = useState([]);
  const [form, setForm] = useState({ code: "", name: "" });
  const [message, setMessage] = useState("");

  async function load() {
    const res = await apiClient.get("/masters/departments");
    setRows(res.data ?? []);
  }

  useEffect(() => {
    load().catch(() => setMessage("Failed to load departments."));
  }, []);

  async function submit(event) {
    event.preventDefault();
    setMessage("");
    try {
      await apiClient.post("/masters/departments", form);
      trackActivity("MASTER_CREATE", `Created department ${form.code}.`);
      setForm({ code: "", name: "" });
      await load();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Department create failed.");
    }
  }

  return (
    <section className="page-card">
      <PageTitle title="Master: Departments" subtitle="Manage department master records." />
      {message ? <p className="error-text">{message}</p> : null}
      <article className="glass-panel">
        <form className="form-grid split" onSubmit={submit}>
          <input placeholder="Department Code" value={form.code} onChange={(e) => setForm((x) => ({ ...x, code: e.target.value }))} required />
          <input placeholder="Department Name" value={form.name} onChange={(e) => setForm((x) => ({ ...x, name: e.target.value }))} required />
          <button type="submit">Add Department</button>
        </form>
      </article>
      <DataGrid columns={[{ key: "code", label: "Code", sortable: true }, { key: "name", label: "Name", sortable: true }, { key: "isActive", label: "Active", sortable: true }]} rows={rows} />
    </section>
  );
}
