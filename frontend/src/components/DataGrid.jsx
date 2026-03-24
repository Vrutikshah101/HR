import { useMemo, useState } from "react";

function defaultAccessor(row, key) {
  const value = row?.[key];
  if (value === null || value === undefined) {
    return "";
  }

  return String(value);
}

export function DataGrid({ columns, rows, emptyText = "No records.", searchPlaceholder = "Search..." }) {
  if (!columns || columns.length === 0) {
    return <p className="info-text">{emptyText}</p>;
  }

  const [query, setQuery] = useState("");
  const [sortKey, setSortKey] = useState(columns[0]?.key ?? "");
  const [sortDirection, setSortDirection] = useState("asc");
  const [filters, setFilters] = useState({});

  const filterableColumns = useMemo(() => columns.filter((col) => col.filterable), [columns]);

  const optionsByColumn = useMemo(() => {
    const result = {};
    for (const column of filterableColumns) {
      const values = Array.from(new Set(rows.map((row) => defaultAccessor(row, column.key)).filter(Boolean)));
      result[column.key] = values.sort((a, b) => a.localeCompare(b));
    }

    return result;
  }, [filterableColumns, rows]);

  const filtered = useMemo(() => {
    const normalizedQuery = query.trim().toLowerCase();

    return rows
      .filter((row) => {
        if (!normalizedQuery) {
          return true;
        }

        return columns.some((col) => defaultAccessor(row, col.key).toLowerCase().includes(normalizedQuery));
      })
      .filter((row) => {
        return Object.entries(filters).every(([key, value]) => {
          if (!value) {
            return true;
          }

          return defaultAccessor(row, key) === value;
        });
      });
  }, [rows, query, columns, filters]);

  const sorted = useMemo(() => {
    if (!sortKey) {
      return filtered;
    }

    const copy = [...filtered];
    copy.sort((a, b) => {
      const left = defaultAccessor(a, sortKey);
      const right = defaultAccessor(b, sortKey);
      return left.localeCompare(right, undefined, { numeric: true, sensitivity: "base" });
    });

    return sortDirection === "asc" ? copy : copy.reverse();
  }, [filtered, sortKey, sortDirection]);

  function onSort(columnKey) {
    if (sortKey === columnKey) {
      setSortDirection((x) => (x === "asc" ? "desc" : "asc"));
      return;
    }

    setSortKey(columnKey);
    setSortDirection("asc");
  }

  return (
    <section className="grid-card">
      <div className="grid-controls">
        <input
          value={query}
          onChange={(event) => setQuery(event.target.value)}
          placeholder={searchPlaceholder}
          aria-label="Search table"
        />

        {filterableColumns.map((column) => (
          <select
            key={column.key}
            value={filters[column.key] ?? ""}
            onChange={(event) => setFilters((x) => ({ ...x, [column.key]: event.target.value }))}
            aria-label={`Filter by ${column.label}`}
          >
            <option value="">All {column.label}</option>
            {(optionsByColumn[column.key] ?? []).map((value) => (
              <option key={value} value={value}>{value}</option>
            ))}
          </select>
        ))}
      </div>

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              {columns.map((column) => (
                <th key={column.key}>
                  {column.sortable ? (
                    <button type="button" className="link-sort" onClick={() => onSort(column.key)}>
                      {column.label}
                      {sortKey === column.key ? (sortDirection === "asc" ? " ▲" : " ▼") : ""}
                    </button>
                  ) : column.label}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {sorted.length === 0 ? (
              <tr>
                <td colSpan={columns.length}>{emptyText}</td>
              </tr>
            ) : (
              sorted.map((row) => (
                <tr key={row.id ?? JSON.stringify(row)}>
                  {columns.map((column) => (
                    <td key={column.key}>
                      {column.render ? column.render(row) : defaultAccessor(row, column.key)}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </section>
  );
}
