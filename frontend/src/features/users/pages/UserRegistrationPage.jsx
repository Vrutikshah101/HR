import { useEffect, useMemo, useState } from "react";
import { DataGrid } from "../../../components/DataGrid";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";
import { trackActivity } from "../../../services/activityTracker";
import { notifyError, notifyInfo } from "../../../services/toast";

export function UserRegistrationPage() {
  const [rows, setRows] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [allDesignations, setAllDesignations] = useState([]);
  const [designationOptions, setDesignationOptions] = useState([]);
  const [message, setMessage] = useState("");
  const [messageKind, setMessageKind] = useState("info");
  const [submitting, setSubmitting] = useState(false);
  const [form, setForm] = useState({
    email: "",
    password: "",
    employeeCode: "",
    firstName: "",
    lastName: "",
    department: "",
    designation: "",
    gender: "",
    dateOfBirth: "",
    joinDate: "",
    dateOfRelieving: "",
    roles: ["User"]
  });

  function normalizeDepartment(item) {
    return {
      id: item?.id ?? item?.departmentId ?? item?.code ?? item?.departmentCode ?? `${item?.name ?? item?.departmentName ?? "dept"}`,
      name: item?.name ?? item?.departmentName ?? item?.label ?? ""
    };
  }

  function normalizeDesignation(item) {
    return {
      id: item?.id ?? item?.designationId ?? item?.code ?? item?.designationCode ?? `${item?.name ?? item?.designationName ?? "desig"}`,
      name: item?.name ?? item?.designationName ?? item?.label ?? ""
    };
  }

  function uniqueByName(rowsToDedupe) {
    const seen = new Set();
    return rowsToDedupe.filter((x) => {
      const key = String(x?.name ?? "").trim().toLowerCase();
      if (!key || seen.has(key)) {
        return false;
      }
      seen.add(key);
      return true;
    });
  }

  function isGuid(value) {
    return /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(value ?? "");
  }

  async function loadData() {
    const [u, d, g] = await Promise.allSettled([
      apiClient.get("/users"),
      apiClient.get("/masters/departments"),
      apiClient.get("/masters/designations")
    ]);

    const users = u.status === "fulfilled" ? (u.value.data ?? []) : [];
    const departmentRows = d.status === "fulfilled" ? uniqueByName((d.value.data ?? []).map(normalizeDepartment).filter((x) => x.name)) : [];
    const designationRows = g.status === "fulfilled" ? uniqueByName((g.value.data ?? []).map(normalizeDesignation).filter((x) => x.name)) : [];

    const fallbackDepartments = uniqueByName(Array.from(new Set(users.map((x) => x?.department).filter(Boolean)))
      .map((name) => ({ id: `user-dept-${name}`, name })));

    const fallbackDesignations = uniqueByName(Array.from(new Set(users.map((x) => x?.designation).filter(Boolean)))
      .map((name) => ({ id: `user-desig-${name}`, name })));

    const defaultDepartments = ["Administration", "Human Resources", "Engineering", "Hardware"]
      .map((name) => ({ id: `default-dept-${name}`, name }));
    const defaultDesignations = ["Administrator", "HR Manager", "Engineering Manager", "Software Engineer", "Assistant Manager", "Executive", "General Manager"]
      .map((name) => ({ id: `default-desig-${name}`, name }));

    const finalDepartments = departmentRows.length > 0 ? departmentRows : (fallbackDepartments.length > 0 ? fallbackDepartments : defaultDepartments);
    const finalDesignations = designationRows.length > 0 ? designationRows : (fallbackDesignations.length > 0 ? fallbackDesignations : defaultDesignations);

    setRows(users);
    setDepartments(finalDepartments);
    setAllDesignations(finalDesignations);
    setDesignationOptions(finalDesignations);

    if (d.status !== "fulfilled" || g.status !== "fulfilled") {
      setMessageKind("info");
      setMessage("Master APIs are unavailable. Showing fallback options.");
      notifyInfo("Master APIs unavailable. Using fallback dropdown values.");
    }
  }

  useEffect(() => {
    loadData().catch((err) => {
      setMessageKind("error");
      const errorMessage = err.response?.data?.message ?? "Failed to load user setup data.";
      setMessage(errorMessage);
      notifyError(errorMessage);
    });
  }, []);

  const fullName = useMemo(() => `${form.firstName} ${form.lastName}`.trim(), [form.firstName, form.lastName]);

  async function onDepartmentChange(departmentName) {
    setForm((x) => ({ ...x, department: departmentName, designation: "" }));

    if (!departmentName) {
      setDesignationOptions(allDesignations);
      return;
    }

    const selectedDepartment = departments.find((x) => x.name === departmentName);
    const departmentId = selectedDepartment?.id;

    if (departmentId && isGuid(departmentId)) {
      try {
        const res = await apiClient.get(`/masters/departments/${departmentId}/designations`);
        const mapped = uniqueByName((res.data ?? []).map(normalizeDesignation).filter((x) => x.name));
        if (mapped.length > 0) {
          setDesignationOptions(mapped);
          return;
        }
      } catch {
        // fallback below
      }
    }

    const fromUsers = uniqueByName(
      rows
        .filter((x) => x.department === departmentName)
        .map((x) => ({ id: `usr-${departmentName}-${x.designation}`, name: x.designation }))
        .filter((x) => x.name)
    );

    setDesignationOptions(fromUsers.length > 0 ? fromUsers : allDesignations);
  }

  async function submit(event) {
    event.preventDefault();
    setMessage("");
    setMessageKind("info");
    setSubmitting(true);

    try {
      await apiClient.post("/users", {
        email: form.email,
        password: form.password,
        employeeCode: form.employeeCode,
        fullName,
        department: form.department,
        designation: form.designation,
        gender: form.gender || null,
        dateOfBirth: form.dateOfBirth || null,
        joinDate: form.joinDate || null,
        dateOfRelieving: form.dateOfRelieving || null,
        roles: form.roles
      });

      trackActivity("USER_CREATE", `User registration has been done successfully for ${form.email}.`);
      setForm({
        email: "",
        password: "",
        employeeCode: "",
        firstName: "",
        lastName: "",
        department: "",
        designation: "",
        gender: "",
        dateOfBirth: "",
        joinDate: "",
        dateOfRelieving: "",
        roles: ["User"]
      });
      setDesignationOptions(allDesignations);
      setMessageKind("info");
      setMessage("User registration has done successfully.");
      await loadData();
    } catch (err) {
      const backendMessage = err.response?.data?.message ?? err.response?.data?.title;
      const status = err.response?.status ? ` (HTTP ${err.response.status})` : "";
      const finalMessage = (backendMessage ?? "User registration failed.") + status;
      setMessageKind("error");
      setMessage(finalMessage);
      notifyError(finalMessage);
    } finally {
      setSubmitting(false);
    }
  }

  function onRoleChange(role) {
    setForm((x) => {
      const has = x.roles.includes(role);
      return { ...x, roles: has ? x.roles.filter((r) => r !== role) : [...x.roles, role] };
    });
  }

  return (
    <section className="page-card">
      <PageTitle title="User Registration" subtitle="Register new users with strict department-designation mapping, gender, DOB, DOJ, and relieving date." />
      {message ? <p className={messageKind === "error" ? "error-text" : "info-text"}>{message}</p> : null}

      <article className="glass-panel">
        <form className="form-grid split" onSubmit={submit}>
          <input placeholder="Employee Code" value={form.employeeCode} onChange={(e) => setForm((x) => ({ ...x, employeeCode: e.target.value }))} required />
          <input type="email" placeholder="Email" value={form.email} onChange={(e) => setForm((x) => ({ ...x, email: e.target.value }))} required />
          <input type="password" placeholder="Password" value={form.password} onChange={(e) => setForm((x) => ({ ...x, password: e.target.value }))} required />
          <input placeholder="First Name" value={form.firstName} onChange={(e) => setForm((x) => ({ ...x, firstName: e.target.value }))} required />
          <input placeholder="Last Name" value={form.lastName} onChange={(e) => setForm((x) => ({ ...x, lastName: e.target.value }))} />
          <input placeholder="Full Name (auto)" value={fullName} readOnly />

          <select value={form.department} onChange={(e) => onDepartmentChange(e.target.value)} required>
            <option value="">Select Department</option>
            {departments.map((d) => <option key={d.id} value={d.name}>{d.name}</option>)}
          </select>

          <select value={form.designation} onChange={(e) => setForm((x) => ({ ...x, designation: e.target.value }))} required>
            <option value="">Select Designation</option>
            {designationOptions.map((d) => <option key={d.id} value={d.name}>{d.name}</option>)}
          </select>

          <select value={form.gender} onChange={(e) => setForm((x) => ({ ...x, gender: e.target.value }))}>
            <option value="">Select Gender</option>
            <option value="MALE">Male</option>
            <option value="FEMALE">Female</option>
            <option value="OTHER">Other</option>
          </select>

          <label>
            Date of Birth
            <input type="date" value={form.dateOfBirth} onChange={(e) => setForm((x) => ({ ...x, dateOfBirth: e.target.value }))} />
          </label>

          <label>
            Date of Joining
            <input type="date" value={form.joinDate} onChange={(e) => setForm((x) => ({ ...x, joinDate: e.target.value }))} required />
          </label>

          <label>
            Date of Relieving
            <input type="date" value={form.dateOfRelieving} onChange={(e) => setForm((x) => ({ ...x, dateOfRelieving: e.target.value }))} />
          </label>

          <div className="full-width role-checks">
            <label><input type="checkbox" checked={form.roles.includes("User")} onChange={() => onRoleChange("User")} /> User</label>
            <label><input type="checkbox" checked={form.roles.includes("Hr")} onChange={() => onRoleChange("Hr")} /> HR</label>
            <label><input type="checkbox" checked={form.roles.includes("Admin")} onChange={() => onRoleChange("Admin")} /> Admin</label>
          </div>

          <button type="submit" disabled={submitting}>{submitting ? "Registering..." : "Register User"}</button>
        </form>
      </article>

      <DataGrid
        columns={[
          { key: "employeeCode", label: "Emp Code", sortable: true },
          { key: "fullName", label: "Name", sortable: true },
          { key: "email", label: "Email", sortable: true },
          { key: "department", label: "Department", sortable: true, filterable: true },
          { key: "designation", label: "Designation", sortable: true, filterable: true },
          { key: "gender", label: "Gender", sortable: true, filterable: true },
          { key: "joinDate", label: "DOJ", sortable: true },
          { key: "dateOfRelieving", label: "DOR", sortable: true },
          { key: "dateOfBirth", label: "DOB", sortable: true },
          { key: "roles", label: "Roles", sortable: false, render: (r) => (r.roles ?? []).join(", ") }
        ]}
        rows={rows}
      />
    </section>
  );
}
