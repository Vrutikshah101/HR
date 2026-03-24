import { useEffect, useState } from "react";
import { DataGrid } from "../../../components/DataGrid";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";
import { trackActivity } from "../../../services/activityTracker";

export function MastersPage() {
  const [departments, setDepartments] = useState([]);
  const [designations, setDesignations] = useState([]);
  const [leaveTypes, setLeaveTypes] = useState([]);
  const [holidays, setHolidays] = useState([]);
  const [message, setMessage] = useState("");

  const [departmentForm, setDepartmentForm] = useState({ code: "", name: "" });
  const [designationForm, setDesignationForm] = useState({ code: "", name: "" });
  const [leaveTypeForm, setLeaveTypeForm] = useState({ code: "", name: "" });
  const [holidayForm, setHolidayForm] = useState({ name: "", date: "", location: "" });

  async function loadAll() {
    const [d, g, l, h] = await Promise.all([
      apiClient.get("/masters/departments"),
      apiClient.get("/masters/designations"),
      apiClient.get("/masters/leave-types"),
      apiClient.get("/masters/holidays")
    ]);

    setDepartments(d.data ?? []);
    setDesignations(g.data ?? []);
    setLeaveTypes(l.data ?? []);
    setHolidays(h.data ?? []);
  }

  useEffect(() => {
    loadAll().catch(() => setMessage("Failed to load masters."));
  }, []);

  async function createDepartment(event) {
    event.preventDefault();
    setMessage("");
    try {
      await apiClient.post("/masters/departments", departmentForm);
      trackActivity("MASTER_CREATE", `Created department ${departmentForm.code}.`);
      setDepartmentForm({ code: "", name: "" });
      await loadAll();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Department create failed.");
    }
  }

  async function createDesignation(event) {
    event.preventDefault();
    setMessage("");
    try {
      await apiClient.post("/masters/designations", designationForm);
      trackActivity("MASTER_CREATE", `Created designation ${designationForm.code}.`);
      setDesignationForm({ code: "", name: "" });
      await loadAll();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Designation create failed.");
    }
  }

  async function createLeaveType(event) {
    event.preventDefault();
    setMessage("");
    try {
      await apiClient.post("/masters/leave-types", {
        ...leaveTypeForm,
        requiresAttachment: false,
        isPaid: true,
        isHalfDayAllowed: true,
        isBackdatedAllowed: false,
        maxDaysPerRequest: null
      });
      trackActivity("MASTER_CREATE", `Created leave type ${leaveTypeForm.code}.`);
      setLeaveTypeForm({ code: "", name: "" });
      await loadAll();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Leave type create failed.");
    }
  }

  async function createHoliday(event) {
    event.preventDefault();
    setMessage("");
    try {
      await apiClient.post("/masters/holidays", {
        ...holidayForm,
        isOptional: false
      });
      trackActivity("MASTER_CREATE", `Created holiday ${holidayForm.name}.`);
      setHolidayForm({ name: "", date: "", location: "" });
      await loadAll();
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Holiday create failed.");
    }
  }

  return (
    <section className="page-card">
      <PageTitle title="Master Data" subtitle="Manage core masters from updated schema: departments, designations, leave types, and holidays." />
      {message ? <p className="error-text">{message}</p> : null}

      <section className="panel-grid">
        <article className="glass-panel">
          <h3>Departments</h3>
          <form className="form-grid split" onSubmit={createDepartment}>
            <input placeholder="Code" value={departmentForm.code} onChange={(e) => setDepartmentForm((x) => ({ ...x, code: e.target.value }))} required />
            <input placeholder="Name" value={departmentForm.name} onChange={(e) => setDepartmentForm((x) => ({ ...x, name: e.target.value }))} required />
            <button type="submit">Add Department</button>
          </form>
          <DataGrid columns={[{ key: "code", label: "Code", sortable: true }, { key: "name", label: "Name", sortable: true }, { key: "isActive", label: "Active", sortable: true }]} rows={departments} />
        </article>

        <article className="glass-panel">
          <h3>Designations</h3>
          <form className="form-grid split" onSubmit={createDesignation}>
            <input placeholder="Code" value={designationForm.code} onChange={(e) => setDesignationForm((x) => ({ ...x, code: e.target.value }))} required />
            <input placeholder="Name" value={designationForm.name} onChange={(e) => setDesignationForm((x) => ({ ...x, name: e.target.value }))} required />
            <button type="submit">Add Designation</button>
          </form>
          <DataGrid columns={[{ key: "code", label: "Code", sortable: true }, { key: "name", label: "Name", sortable: true }, { key: "isActive", label: "Active", sortable: true }]} rows={designations} />
        </article>
      </section>

      <section className="panel-grid">
        <article className="glass-panel">
          <h3>Leave Types</h3>
          <form className="form-grid split" onSubmit={createLeaveType}>
            <input placeholder="Code" value={leaveTypeForm.code} onChange={(e) => setLeaveTypeForm((x) => ({ ...x, code: e.target.value }))} required />
            <input placeholder="Name" value={leaveTypeForm.name} onChange={(e) => setLeaveTypeForm((x) => ({ ...x, name: e.target.value }))} required />
            <button type="submit">Add Leave Type</button>
          </form>
          <DataGrid columns={[{ key: "code", label: "Code", sortable: true }, { key: "name", label: "Name", sortable: true }, { key: "isActive", label: "Active", sortable: true }]} rows={leaveTypes} />
        </article>

        <article className="glass-panel">
          <h3>Holidays</h3>
          <form className="form-grid split" onSubmit={createHoliday}>
            <input placeholder="Name" value={holidayForm.name} onChange={(e) => setHolidayForm((x) => ({ ...x, name: e.target.value }))} required />
            <input type="date" value={holidayForm.date} onChange={(e) => setHolidayForm((x) => ({ ...x, date: e.target.value }))} required />
            <input placeholder="Location (optional)" value={holidayForm.location} onChange={(e) => setHolidayForm((x) => ({ ...x, location: e.target.value }))} />
            <button type="submit">Add Holiday</button>
          </form>
          <DataGrid columns={[{ key: "name", label: "Name", sortable: true }, { key: "date", label: "Date", sortable: true }, { key: "location", label: "Location", sortable: true }]} rows={holidays} />
        </article>
      </section>
    </section>
  );
}
