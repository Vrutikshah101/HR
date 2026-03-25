import { useEffect, useState } from "react";
import { PageTitle } from "../../../components/PageTitle";
import { StatGrid } from "../../../components/StatGrid";
import { DashboardCharts } from "../../../components/charts/DashboardCharts";
import { apiClient } from "../../../services/apiClient";

export function EmployeeDashboardPage() {
  const [cards, setCards] = useState([]);
  const [profile, setProfile] = useState({ fullName: "User", designation: "-", department: "-", email: "" });

  useEffect(() => {
    apiClient.get("/dashboard/employee")
      .then((res) => {
        const mapped = (res.data ?? []).map((item) => ({
          key: item.key,
          label: item.label,
          value: String(item.value),
          hint: "Live metric",
          delta: ""
        }));

        setCards(mapped);
      })
      .catch(() => setCards([]));

    apiClient.get("/users/me")
      .then((res) => {
        setProfile({
          fullName: res.data?.fullName ?? "User",
          designation: res.data?.designation ?? "-",
          department: res.data?.department ?? "-",
          email: res.data?.email ?? ""
        });
      })
      .catch(() => setProfile({ fullName: "User", designation: "-", department: "-", email: "" }));
  }, []);

  return (
    <section className="page-card">
      <PageTitle title="Employee Module" subtitle="Overview of leave account, pending tasks, and key notifications." />

      <section className="dashboard-layout">
        <article className="profile-pane">
          <div className="profile-pane-head">Person Profile</div>
          <div className="profile-photo">{profile.fullName.slice(0, 1).toUpperCase()}</div>
          <h3>{profile.fullName}</h3>
          <p>{profile.designation}</p>
          <p>{profile.department}</p>
          <p>{profile.email}</p>
        </article>

        <section className="dashboard-main">
          <div className="hero-cards">
            <article className="hero-card tone-orange">
              <h4>Leave Attendance</h4>
              <strong>{cards[0]?.value ?? "0"}</strong>
              <span>{cards[0]?.label ?? "Balance"}</span>
            </article>
            <article className="hero-card tone-cyan">
              <h4>Compensation</h4>
              <strong>{cards[1]?.value ?? "0"}</strong>
              <span>{cards[1]?.label ?? "Compensation"}</span>
            </article>
            <article className="hero-card tone-pink">
              <h4>Appraisal Score</h4>
              <strong>{cards[2]?.value ?? "0"}</strong>
              <span>{cards[2]?.label ?? "Rating"}</span>
            </article>
          </div>

          <article className="glass-panel">
            <h3>Pending Task Details</h3>
            <StatGrid items={cards} />
          </article>

          <DashboardCharts cards={cards} />
        </section>
      </section>
    </section>
  );
}
