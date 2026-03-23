export function UnauthorizedPage({ role }) {
  return (
    <section className="page-card">
      <h1>Access Restricted</h1>
      <p>This page is not visible for the active role: <strong>{role}</strong>.</p>
      <p>Use the role switcher in the header to preview permitted screens.</p>
    </section>
  );
}
