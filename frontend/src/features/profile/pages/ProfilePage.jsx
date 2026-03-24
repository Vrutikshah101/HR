import { useEffect, useMemo, useState } from "react";
import { PageTitle } from "../../../components/PageTitle";
import { apiClient } from "../../../services/apiClient";
import { getRecentActivities, trackActivity } from "../../../services/activityTracker";

function toInitials(fullName, email) {
  const source = (fullName || email || "U").trim();
  const parts = source.split(/\s+/).filter(Boolean);
  return parts.slice(0, 2).map((x) => x[0]?.toUpperCase() ?? "").join("") || "U";
}

export function ProfilePage() {
  const [form, setForm] = useState({
    email: "",
    employeeCode: "",
    fullName: "",
    department: "",
    designation: "",
    roles: []
  });
  const [activities, setActivities] = useState([]);
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);

  async function load() {
    const response = await apiClient.get("/users/me");
    const data = response.data;
    setForm({
      email: data.email ?? "",
      employeeCode: data.employeeCode ?? "",
      fullName: data.fullName ?? "",
      department: data.department ?? "",
      designation: data.designation ?? "",
      roles: data.roles ?? []
    });
    setActivities(getRecentActivities(30));
  }

  useEffect(() => {
    trackActivity("PAGE_VISIT", "Opened Profile page.");
    load().catch(() => setMessage("Failed to load profile."));

    function refresh() {
      setActivities(getRecentActivities(30));
    }

    window.addEventListener("lms:activity-updated", refresh);
    window.addEventListener("focus", refresh);

    return () => {
      window.removeEventListener("lms:activity-updated", refresh);
      window.removeEventListener("focus", refresh);
    };
  }, []);

  const initials = useMemo(() => toInitials(form.fullName, form.email), [form.fullName, form.email]);

  async function save(event) {
    event.preventDefault();
    setLoading(true);
    setMessage("");

    try {
      await apiClient.put("/users/me", {
        fullName: form.fullName,
        department: form.department,
        designation: form.designation
      });
      trackActivity("PROFILE_UPDATE", "Updated profile details.");
      setActivities(getRecentActivities(30));
      setMessage("Profile updated.");
    } catch (err) {
      setMessage(err.response?.data?.message ?? "Profile update failed.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="page-card">
      <PageTitle title="Profile" subtitle="Review your account details and update your basic information." />
      {message ? <p className="info-text">{message}</p> : null}

      <section className="panel-grid">
        <article className="glass-panel profile-overview">
          <div className="profile-avatar">{initials}</div>
          <div className="profile-meta">
            <h3>{form.fullName || "User"}</h3>
            <p>{form.email}</p>
            <p>Employee Code: {form.employeeCode || "-"}</p>
            <div className="flag-row">
              {form.roles.map((role) => <span key={role} className="flag green">{role}</span>)}
            </div>
          </div>
        </article>

        <article className="glass-panel">
          <h3>Edit Profile</h3>
          <form className="form-grid" onSubmit={save}>
            <label>
              Full Name
              <input value={form.fullName} onChange={(e) => setForm((x) => ({ ...x, fullName: e.target.value }))} required />
            </label>
            <label>
              Department
              <input value={form.department} onChange={(e) => setForm((x) => ({ ...x, department: e.target.value }))} required />
            </label>
            <label>
              Designation
              <input value={form.designation} onChange={(e) => setForm((x) => ({ ...x, designation: e.target.value }))} required />
            </label>
            <button type="submit" disabled={loading}>{loading ? "Saving..." : "Save Profile"}</button>
          </form>
        </article>
      </section>

      <article className="glass-panel">
        <h3>Recent Activity</h3>
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>When</th>
                <th>Type</th>
                <th>Details</th>
              </tr>
            </thead>
            <tbody>
              {activities.length === 0 ? (
                <tr><td colSpan="3">No activity captured yet.</td></tr>
              ) : activities.map((item) => (
                <tr key={item.id}>
                  <td>{new Date(item.at).toLocaleString()}</td>
                  <td>{item.type}</td>
                  <td>{item.message}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </article>
    </section>
  );
}
