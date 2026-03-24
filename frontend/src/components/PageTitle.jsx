export function PageTitle({ title, subtitle }) {
  return (
    <header className="page-title">
      <div className="page-title-top">
        <span className="section-dot" aria-hidden="true" />
        <p className="page-kicker">Workspace</p>
      </div>
      <h1>{title}</h1>
      <p>{subtitle}</p>
    </header>
  );
}
