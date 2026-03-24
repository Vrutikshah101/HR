import { useEffect, useMemo, useState } from "react";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";
import { trackActivity } from "../../../services/activityTracker";

export function HierarchySetupPage() {
  const [users, setUsers] = useState([]);
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);
  const [filterDepartment, setFilterDepartment] = useState("");
  const [employeeId, setEmployeeId] = useState("");
  const [level1ApproverEmployeeId, setLevel1ApproverEmployeeId] = useState("");
  const [level2ApproverEmployeeId, setLevel2ApproverEmployeeId] = useState("");

  async function loadUsers() {
    const response = await apiClient.get("/users");
    setUsers(response.data ?? []);
  }

  useEffect(() => {
    loadUsers().catch((err) => setMessage(err.response?.data?.message ?? "Failed to load users for hierarchy setup."));
  }, []);

  const departments = useMemo(() => {
    return Array.from(new Set(users.map((x) => x.department).filter(Boolean))).sort();
  }, [users]);

  const filteredUsers = useMemo(() => {
    if (!filterDepartment) {
      return users;
    }

    return users.filter((x) => x.department === filterDepartment);
  }, [filterDepartment, users]);

  const selectedEmployee = useMemo(() => users.find((x) => x.employeeId === employeeId) ?? null, [employeeId, users]);

  const approverOptions = useMemo(() => {
    const scoped = filterDepartment
      ? users.filter((x) => x.department === filterDepartment)
      : users;

    return scoped.filter((x) => x.employeeId !== employeeId);
  }, [employeeId, filterDepartment, users]);

  useEffect(() => {
    if (!employeeId) {
      setLevel1ApproverEmployeeId("");
      setLevel2ApproverEmployeeId("");
      return;
    }

    apiClient.get(`/hierarchy/${employeeId}`)
      .then((res) => {
        setLevel1ApproverEmployeeId(res.data?.level1ApproverEmployeeId ?? "");
        setLevel2ApproverEmployeeId(res.data?.level2ApproverEmployeeId ?? "");
      })
      .catch((err) => {
        if (err?.response?.status === 404) {
          setLevel1ApproverEmployeeId("");
          setLevel2ApproverEmployeeId("");
          return;
        }

        setMessage(err.response?.data?.message ?? "Failed to load current hierarchy.");
      });
  }, [employeeId]);

  async function saveHierarchy(event) {
    event.preventDefault();
    setMessage("");
    setLoading(true);

    try {
      await apiClient.put("/hierarchy", {
        employeeId,
        level1ApproverEmployeeId: level1ApproverEmployeeId || null,
        level2ApproverEmployeeId: level2ApproverEmployeeId || null
      });

      trackActivity("HIERARCHY_UPDATE", `Updated hierarchy for ${selectedEmployee?.fullName ?? employeeId}.`, {
        employeeId,
        level1ApproverEmployeeId,
        level2ApproverEmployeeId
      });
      setMessage("Hierarchy workflow updated successfully.");
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Failed to save hierarchy workflow.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="page-card">
      <PageTitle title="Hierarchy Workflow" subtitle="Configure reporting/reviewing officers by department and user." />
      {message ? <p className="info-text">{message}</p> : null}

      <article className="glass-panel">
        <form className="form-grid split" onSubmit={saveHierarchy}>
          <select value={filterDepartment} onChange={(e) => setFilterDepartment(e.target.value)}>
            <option value="">All Departments</option>
            {departments.map((d) => <option key={d} value={d}>{d}</option>)}
          </select>

          <select value={employeeId} onChange={(e) => setEmployeeId(e.target.value)} required>
            <option value="">Select Employee</option>
            {filteredUsers.map((u) => (
              <option key={u.employeeId} value={u.employeeId}>
                {u.fullName} ({u.email})
              </option>
            ))}
          </select>

          <select value={level1ApproverEmployeeId} onChange={(e) => setLevel1ApproverEmployeeId(e.target.value)} disabled={!employeeId}>
            <option value="">Select Level 1 Approver</option>
            {approverOptions.map((u) => (
              <option key={`l1-${u.employeeId}`} value={u.employeeId}>
                {u.fullName} ({u.email})
              </option>
            ))}
          </select>

          <select value={level2ApproverEmployeeId} onChange={(e) => setLevel2ApproverEmployeeId(e.target.value)} disabled={!employeeId}>
            <option value="">Select Level 2 Approver</option>
            {approverOptions.map((u) => (
              <option key={`l2-${u.employeeId}`} value={u.employeeId}>
                {u.fullName} ({u.email})
              </option>
            ))}
          </select>

          <button type="submit" disabled={!employeeId || loading}>{loading ? "Saving..." : "Save Workflow"}</button>
        </form>
      </article>
    </section>
  );
}
