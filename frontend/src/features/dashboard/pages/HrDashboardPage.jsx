import { useEffect, useState } from "react";
import { PageTitle } from "../../../components/PageTitle";
import { StatGrid } from "../../../components/StatGrid";
import { apiClient } from "../../../services/apiClient";

export function HrDashboardPage() {
  const [cards, setCards] = useState([]);

  useEffect(() => {
    apiClient.get("/dashboard/hr")
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
      <PageTitle title="HR Dashboard" subtitle="Live HR KPIs from backend." />
      <StatGrid items={cards} />
    </section>
  );
}
