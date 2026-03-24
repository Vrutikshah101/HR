import { useEffect, useState } from "react";
import { DataGrid } from "../../../components/DataGrid";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";
import { trackActivity } from "../../../services/activityTracker";

export function DepartmentDesignationMapPage() {
  const [departments, setDepartments] = useState([]);
  const [designations, setDesignations] = useState([]);
  const [mappings, setMappings] = useState([]);
  const [message, setMessage] = useState("");
  const [form, setForm] = useState({
    departmentId: "",
    designationId: ""
  });

  async function load() {
    const [depRes, desRes, mapRes] = await Promise.all([
      apiClient.get("/masters/departments"),
      apiClient.get("/masters/designations"),
      apiClient.get("/masters/department-designation-maps")
    ]);

    setDepartments(depRes.data ?? []);
    setDesignations(desRes.data ?? []);
    setMappings(mapRes.data ?? []);
  }

  useEffect(() => {
    load().catch((err) => setMessage(err.response?.data?.message ?? "Failed to load mapping masters."));
  }, []);

  async function submit(event) {
    event.preventDefault();
    setMessage("");

    try {
      const res = await apiClient.post("/masters/department-designation-maps", form);
      trackActivity("MASTER_CREATE", `Mapped ${res.data?.departmentName} -> ${res.data?.designationName}`);
      setForm({ departmentId: "", designationId: "" });
      setMessage("Mapping created successfully.");
      await load();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Failed to create mapping.");
    }
  }

  return (
    <section className="page-card">
      <PageTitle title="Department-Designation Mapping" subtitle="Define strict designation options for each department." />
      {message ? <p className="info-text">{message}</p> : null}

      <article className="glass-panel">
        <form className="form-grid split" onSubmit={submit}>
          <select value={form.departmentId} onChange={(e) => setForm((x) => ({ ...x, departmentId: e.target.value }))} required>
            <option value="">Select Department</option>
            {(departments ?? []).map((d) => <option key={d.id} value={d.id}>{d.name}</option>)}
          </select>

          <select value={form.designationId} onChange={(e) => setForm((x) => ({ ...x, designationId: e.target.value }))} required>
            <option value="">Select Designation</option>
            {(designations ?? []).map((d) => <option key={d.id} value={d.id}>{d.name}</option>)}
          </select>

          <button type="submit">Add Mapping</button>
        </form>
      </article>

      <DataGrid
        columns={[
          { key: "departmentName", label: "Department", sortable: true, filterable: true },
          { key: "designationName", label: "Designation", sortable: true, filterable: true }
        ]}
        rows={mappings}
      />
    </section>
  );
}
