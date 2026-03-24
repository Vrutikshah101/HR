import { useEffect, useState } from "react";
import { PageTitle } from "../../../components/PageTitle";
import { getRecentActivities } from "../../../services/activityTracker";

export function ActivityTrackerPage() {
  const [activities, setActivities] = useState([]);

  useEffect(() => {
    function refresh() {
      setActivities(getRecentActivities(100));
    }

    refresh();
    window.addEventListener("lms:activity-updated", refresh);
    window.addEventListener("focus", refresh);

    return () => {
      window.removeEventListener("lms:activity-updated", refresh);
      window.removeEventListener("focus", refresh);
    };
  }, []);

  return (
    <section className="page-card">
      <PageTitle title="Activity Tracker" subtitle="Recent page visits and actions captured in the application." />

      <article className="glass-panel">
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
