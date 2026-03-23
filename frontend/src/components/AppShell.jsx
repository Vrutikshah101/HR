import { NavLink, Outlet } from "react-router-dom";
import { getMenuItems, mapPrimaryRole } from "../app/roles";

const roleTag = {
  user: "Employee",
  hr: "HR",
  admin: "Admin"
};

export function AppShell({ roles, onLogout }) {
  const primaryRole = mapPrimaryRole(roles);
  const menuItems = getMenuItems(roles);

  return (
    <div className="shell">
      <header className="shell-header">
        <div className="header-left">
          <p className="eyebrow">Leave Management System</p>
          <h1>Workspace</h1>
          <div className="header-chips">
            <span className="chip chip-highlight">{roleTag[primaryRole]} Access</span>
            <span className="chip">Live API Mode</span>
          </div>
        </div>

        <button type="button" className="secondary" onClick={onLogout}>Logout</button>
      </header>

      <div className="shell-content">
        <aside className="shell-nav">
          <p className="nav-title">Navigation</p>
          {menuItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) => `nav-item${isActive ? " active" : ""}`}
            >
              {item.label}
            </NavLink>
          ))}
        </aside>

        <section className="shell-main">
          <Outlet />
        </section>
      </div>
    </div>
  );
}
