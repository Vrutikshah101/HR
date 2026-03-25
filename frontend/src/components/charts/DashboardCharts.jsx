import {
  Bar,
  BarChart,
  Cell,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis
} from "recharts";

const palette = ["#1d4ed8", "#0ea5e9", "#22c55e", "#f59e0b", "#ef4444", "#7c3aed"];

function parseCards(cards) {
  return (cards ?? []).map((card, idx) => ({
    key: card.key ?? `kpi-${idx}`,
    label: card.label ?? `KPI ${idx + 1}`,
    value: Number(card.value ?? 0)
  }));
}

export function DashboardCharts({ cards }) {
  const data = parseCards(cards).filter((x) => Number.isFinite(x.value));

  if (!data.length) {
    return null;
  }

  return (
    <section className="chart-grid" aria-label="KPI charts">
      <article className="chart-card">
        <header className="chart-head">KPI Bar Chart</header>
        <div className="chart-body">
          <ResponsiveContainer width="100%" height={260}>
            <BarChart data={data} margin={{ left: 4, right: 8, top: 12, bottom: 0 }}>
              <XAxis dataKey="label" tick={{ fontSize: 11 }} interval={0} angle={-10} height={52} />
              <YAxis allowDecimals={false} tick={{ fontSize: 11 }} />
              <Tooltip />
              <Bar dataKey="value" radius={[6, 6, 0, 0]}>
                {data.map((entry, idx) => (
                  <Cell key={entry.key} fill={palette[idx % palette.length]} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </div>
      </article>

      <article className="chart-card">
        <header className="chart-head">KPI Distribution</header>
        <div className="chart-body">
          <ResponsiveContainer width="100%" height={260}>
            <PieChart>
              <Tooltip />
              <Pie data={data} dataKey="value" nameKey="label" cx="50%" cy="50%" outerRadius={92} innerRadius={50}>
                {data.map((entry, idx) => (
                  <Cell key={entry.key} fill={palette[idx % palette.length]} />
                ))}
              </Pie>
            </PieChart>
          </ResponsiveContainer>
        </div>
      </article>
    </section>
  );
}
