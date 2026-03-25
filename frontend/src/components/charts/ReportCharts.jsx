import { useMemo } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis
} from "recharts";

const palette = ["#1d4ed8", "#0ea5e9", "#22c55e", "#f59e0b", "#ef4444", "#7c3aed", "#db2777"];

function tryNumber(value) {
  if (typeof value === "number") {
    return value;
  }

  if (typeof value === "string") {
    const n = Number(value);
    return Number.isFinite(n) ? n : null;
  }

  return null;
}

export function ReportCharts({ rows }) {
  const chartData = useMemo(() => {
    if (!rows?.length) {
      return [];
    }

    const keys = Object.keys(rows[0]);
    const numericKeys = keys.filter((key) => rows.some((r) => tryNumber(r[key]) !== null));
    const labelKey = keys.find((key) => !numericKeys.includes(key)) ?? keys[0];
    const numericKey = numericKeys[0];

    if (!numericKey) {
      return [];
    }

    return rows.slice(0, 20).map((row, idx) => ({
      name: String(row[labelKey] ?? `Row ${idx + 1}`),
      value: tryNumber(row[numericKey]) ?? 0
    }));
  }, [rows]);

  if (!chartData.length) {
    return null;
  }

  return (
    <section className="chart-grid" aria-label="Report charts">
      <article className="chart-card">
        <header className="chart-head">Report Trend</header>
        <div className="chart-body">
          <ResponsiveContainer width="100%" height={260}>
            <BarChart data={chartData} margin={{ top: 8, right: 8, left: 0, bottom: 24 }}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" tick={{ fontSize: 10 }} interval={0} angle={-20} textAnchor="end" height={56} />
              <YAxis allowDecimals={false} tick={{ fontSize: 11 }} />
              <Tooltip />
              <Bar dataKey="value" radius={[6, 6, 0, 0]}>
                {chartData.map((entry, idx) => (
                  <Cell key={`${entry.name}-${idx}`} fill={palette[idx % palette.length]} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </div>
      </article>

      <article className="chart-card">
        <header className="chart-head">Report Distribution</header>
        <div className="chart-body">
          <ResponsiveContainer width="100%" height={260}>
            <PieChart>
              <Tooltip />
              <Pie data={chartData} dataKey="value" nameKey="name" innerRadius={52} outerRadius={95}>
                {chartData.map((entry, idx) => (
                  <Cell key={`${entry.name}-${idx}`} fill={palette[idx % palette.length]} />
                ))}
              </Pie>
            </PieChart>
          </ResponsiveContainer>
        </div>
      </article>
    </section>
  );
}
