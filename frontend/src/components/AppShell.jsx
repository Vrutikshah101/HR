import { NavLink, Outlet, useLocation } from "react-router-dom";
import { useEffect, useMemo, useState } from "react";
import { getMenuItems, mapPrimaryRole } from "../app/roles";
import { apiClient } from "../services/apiClient";
import { Breadcrumbs } from "./Breadcrumbs";
import { trackActivity, trackPageVisit } from "../services/activityTracker";
import { ApprovalIcon, DashboardIcon, LeavesIcon, LogoutIcon, ReportIcon, UserIcon } from "./icons";

const roleTag = {
  user: "Employee",
  hr: "HR",
  admin: "Admin"
};

const iconByKey = {
  dashboard: DashboardIcon,
  leaves: LeavesIcon,
  approvals: ApprovalIcon,
  reports: ReportIcon,
  profile: UserIcon
};

export function AppShell({ roles, onLogout }) {
  const location = useLocation();
  const primaryRole = mapPrimaryRole(roles);
  const menuItems = getMenuItems(roles);
  const [profile, setProfile] = useState({ fullName: "User", email: "", employeeCode: "" });
  const [menuPinned, setMenuPinned] = useState(false);
  const [menuOpenMobile, setMenuOpenMobile] = useState(false);

  useEffect(() => {
    apiClient.get("/users/me")
      .then((res) => {
        setProfile({
          fullName: res.data?.fullName ?? "User",
          email: res.data?.email ?? "",
          employeeCode: res.data?.employeeCode ?? ""
        });
      })
      .catch(() => {
        setProfile({ fullName: "User", email: "", employeeCode: "" });
      });
  }, []);

  useEffect(() => {
    trackPageVisit(location.pathname);
    setMenuOpenMobile(false);
  }, [location.pathname]);

  const initials = useMemo(() => {
    const parts = (profile.fullName || profile.email || "U").split(/\s+/).filter(Boolean);
    return parts.slice(0, 2).map((x) => x[0]?.toUpperCase() ?? "").join("") || "U";
  }, [profile.email, profile.fullName]);

  return (
    <div className="shell">
      <header className="shell-header">
        <div className="header-left modern-headline">
          <p className="eyebrow">Leave Management System</p>
          <h1>Control Center</h1>
          <div className="header-chips">
            <span className="chip chip-highlight">{roleTag[primaryRole]} Access</span>
            <span className="chip">Live API Mode</span>
            <span className="chip chip-soft">{profile.employeeCode || "Profile Ready"}</span>
          </div>
        </div>

        <div className="top-user-menu top-actions">
          <button type="button" className="secondary menu-toggle" onClick={() => setMenuOpenMobile((x) => !x)}>
            ☰ Menu
          </button>

          <button
            type="button"
            className={`secondary menu-toggle ${menuPinned ? "active" : ""}`}
            onClick={() => setMenuPinned((x) => !x)}
            title={menuPinned ? "Unpin menu" : "Pin menu"}
          >
            {menuPinned ? "Pinned" : "Auto Hide"}
          </button>

          <div className="avatar-badge" aria-hidden="true">{initials}</div>
          <div className="user-text">
            <strong>{profile.fullName}</strong>
            <span>{profile.email}</span>
          </div>
          <button
            type="button"
            className="secondary"
            onClick={() => {
              trackActivity("LOGOUT", "User signed out.");
              onLogout();
            }}
          >
            <LogoutIcon width="16" height="16" />
            Sign Out
          </button>
        </div>
      </header>

      <div className={`shell-content ${menuPinned ? "menu-pinned" : "menu-auto"} ${menuOpenMobile ? "menu-open-mobile" : ""}`}>
        <aside className="shell-nav">
          <p className="nav-title">Navigation</p>
          {menuItems.map((item) => (
            <NavLink key={item.to} to={item.to} className={({ isActive }) => `nav-item${isActive ? " active" : ""}`}>
              {(() => {
                const Icon = iconByKey[item.icon] ?? DashboardIcon;
                return <Icon width="16" height="16" className="nav-icon" />;
              })()}
              <span className="nav-label">{item.label}</span>
            </NavLink>
          ))}
        </aside>

        <section className="shell-main">
          <Breadcrumbs />
          <Outlet />
        </section>
      </div>

      <footer className="shell-footer">
        <span>Leave Management System</span>
        <span>Role-Based Workspace</span>
        <span>{new Date().getFullYear()}</span>
      </footer>
    </div>
  );
}
