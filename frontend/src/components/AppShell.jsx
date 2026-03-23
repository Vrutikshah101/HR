import { NavLink, Outlet } from "react-router-dom";
import { menuItems, roleOptions } from "../app/roles";

const roleTag = {
  employee: "Employee View",
  approver: "Approver View",
  hr: "HR Operations",
  admin: "Admin Control"
};

export function AppShell({ role, onRoleChange }) {
  const allowedMenu = menuItems.filter((item) => item.roles.includes(role));

  return (
    <div className="shell">
      <header className="shell-header">
        <div className="header-left">
          <p className="eyebrow">Leave Management System</p>
          <h1>Modern Workspace Preview</h1>
          <div className="header-chips">
            <span className="chip">Phase 1 Static UI</span>
            <span className="chip chip-soft">No Backend Logic</span>
            <span className="chip chip-highlight">{roleTag[role]}</span>
          </div>
        </div>

        <label className="role-picker">
          Active Role
          <select value={role} onChange={(event) => onRoleChange(event.target.value)}>
            {roleOptions.map((roleOption) => (
              <option key={roleOption.value} value={roleOption.value}>
                {roleOption.label}
              </option>
            ))}
          </select>
        </label>
      </header>

      <div className="shell-content">
        <aside className="shell-nav">
          <p className="nav-title">Navigation</p>
          {allowedMenu.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) => `nav-item${isActive ? " active" : ""}`}
            >
              {item.label}
            </NavLink>
          ))}
          <div className="nav-note">Role-based menus are mocked for Phase 1 usability validation.</div>
        </aside>

        <section className="shell-main">
          <Outlet />
        </section>
      </div>
    </div>
  );
}
