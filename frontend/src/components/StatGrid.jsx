export function StatGrid({ items }) {
  return (
    <section className="stat-grid">
      {items.map((item, index) => (
        <article className={`stat-card stat-tone-${(index % 4) + 1}`} key={item.key}>
          <div className="stat-head">
            <h2>{item.label}</h2>
            {item.delta ? <span className="delta-pill">{item.delta}</span> : null}
          </div>
          <strong>{item.value}</strong>
          <span>{item.hint}</span>
        </article>
      ))}
    </section>
  );
}
