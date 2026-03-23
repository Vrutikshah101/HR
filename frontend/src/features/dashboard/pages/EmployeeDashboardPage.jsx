import { useEffect, useState } from "react";
import { PageTitle } from "../../../components/PageTitle";
import { StatGrid } from "../../../components/StatGrid";
import { apiClient } from "../../../services/apiClient";

export function EmployeeDashboardPage() {
  const [cards, setCards] = useState([]);

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
  }, []);

  return (
    <section className="page-card">
      <PageTitle title="Employee Dashboard" subtitle="Live role dashboard from backend metrics." />
      <StatGrid items={cards} />
    </section>
  );
}
