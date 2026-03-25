import DataGrid from "../../../components/DataGrid";

const columns = [
  { key: "name", title: "Name" },
  { key: "status", title: "Status" }
];

const rows = [
  { name: "Sample Row", status: "Active" }
];

export default function SamplePage() {
  return (
    <section>
      <h2>Sample Feature Page</h2>
      <DataGrid columns={columns} rows={rows} />
    </section>
  );
}
