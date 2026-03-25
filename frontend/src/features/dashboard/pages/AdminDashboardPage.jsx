import { useEffect, useState } from "react";
import { PageTitle } from "../../../components/PageTitle";
import { StatGrid } from "../../../components/StatGrid";
import { DashboardCharts } from "../../../components/charts/DashboardCharts";
import { apiClient } from "../../../services/apiClient";

export function AdminDashboardPage() {
  const [cards, setCards] = useState([]);

  useEffect(() => {
    apiClient.get("/dashboard/admin")
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
      <PageTitle title="Admin Dashboard" subtitle="Live admin KPIs from backend." />
      <StatGrid items={cards} />
      <DashboardCharts cards={cards} />
    </section>
  );
}
