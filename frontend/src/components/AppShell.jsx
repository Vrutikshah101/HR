import { NavLink, Outlet, useLocation } from "react-router-dom";
import { useEffect, useMemo, useState } from "react";
import { getMenuItems, mapPrimaryRole } from "../app/roles";
import { apiClient } from "../services/apiClient";
import { Breadcrumbs } from "./Breadcrumbs";
import { ToastHost } from "./ToastHost";
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
  const [openGroup, setOpenGroup] = useState(null);

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
    setOpenGroup(null);
  }, [location.pathname]);

  const initials = useMemo(() => {
    const parts = (profile.fullName || profile.email || "U").split(/\s+/).filter(Boolean);
    return parts.slice(0, 2).map((x) => x[0]?.toUpperCase() ?? "").join("") || "U";
  }, [profile.email, profile.fullName]);

  return (
    <div className="shell gov-shell">
      <header className="gov-topbar">
        <div className="gov-brand-left">
          <div className="state-mark">GJ</div>
          <div className="brand-lines">
            <strong>Karmyogi Leave Portal</strong>
            <span>Government Workflow Suite</span>
          </div>
        </div>

        <nav className="gov-main-menu">
          {menuItems.map((item) => {
            const Icon = iconByKey[item.icon] ?? DashboardIcon;

            if (item.children?.length) {
              const isParentActive = item.children.some((child) => location.pathname.startsWith(child.to));
              return (
                <div key={item.label} className={`gov-menu-group${isParentActive ? " active" : ""} ${openGroup === item.label ? "open" : ""}`}>
                  <button
                    type="button"
                    className="gov-menu-link gov-menu-parent"
                    onClick={() => setOpenGroup((x) => (x === item.label ? null : item.label))}
                  >
                    <Icon width="14" height="14" className="nav-icon" />
                    <span>{item.label}</span>
                  </button>
                  <div className="gov-menu-dropdown">
                    {item.children.map((child) => (
                      <NavLink key={child.to} to={child.to} className={({ isActive }) => `gov-menu-child${isActive ? " active" : ""}`}>
                        {child.label}
                      </NavLink>
                    ))}
                  </div>
                </div>
              );
            }

            return (
              <NavLink key={item.to} to={item.to} className={({ isActive }) => `gov-menu-link${isActive ? " active" : ""}`}>
                <Icon width="14" height="14" className="nav-icon" />
                <span>{item.label}</span>
              </NavLink>
            );
          })}
        </nav>

        <div className="gov-user-strip">
          <span className="gov-role">{roleTag[primaryRole]}</span>
          <div className="avatar-badge" aria-hidden="true">{initials}</div>
          <div className="user-text">
            <strong>{profile.fullName}</strong>
            <span>{profile.employeeCode || profile.email}</span>
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
          </button>
        </div>
      </header>

      <div className="shell-content">
        <section className="shell-main">
          <div className="gov-content-header">
            <h2 className="gov-page-head">Leave Information</h2>
            <Breadcrumbs />
          </div>
          <Outlet />
        </section>
      </div>

      <footer className="shell-footer">
        <div className="footer-pill footer-brand">Leave Management System</div>
        <div className="footer-pill footer-contact">Email: support@leave.local</div>
        <div className="footer-pill footer-contact">Helpdesk: +91-79-23258575</div>
        <div className="footer-pill footer-year">{new Date().getFullYear()}</div>
      </footer>
      <ToastHost />
    </div>
  );
}
