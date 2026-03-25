export default function DataGrid({ columns, rows }) {
  return (
    <table className="grid">
      <thead>
        <tr>{columns.map((col) => <th key={col.key}>{col.title}</th>)}</tr>
      </thead>
      <tbody>
        {rows.map((row, idx) => (
          <tr key={idx}>
            {columns.map((col) => <td key={col.key}>{row[col.key]}</td>)}
          </tr>
        ))}
      </tbody>
    </table>
  );
}
