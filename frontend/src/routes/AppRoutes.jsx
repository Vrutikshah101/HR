import { Navigate, Route, Routes } from "react-router-dom";
import { AppShell } from "../components/AppShell";
import { UnauthorizedPage } from "../components/UnauthorizedPage";
import { defaultRouteByRole, menuItems } from "../app/roles";
import { LoginPage } from "../features/auth/pages/LoginPage";
import { EmployeeLeavePage } from "../features/leaves/pages/EmployeeLeavePage";
import { ApprovalsPage } from "../features/approvals/pages/ApprovalsPage";
import { EmployeeDashboardPage } from "../features/dashboard/pages/EmployeeDashboardPage";
import { HrDashboardPage } from "../features/dashboard/pages/HrDashboardPage";
import { AdminDashboardPage } from "../features/dashboard/pages/AdminDashboardPage";
import { LeaveBalanceReportPage } from "../features/reports/pages/LeaveBalanceReportPage";
import { useEffect, useMemo, useState } from "react";

function RoleGate({ role, allowedRoles, children }) {
  if (!allowedRoles.includes(role)) {
    return <UnauthorizedPage role={role} />;
  }

  return children;
}

export function AppRoutes() {
  const [role, setRole] = useState(() => localStorage.getItem("phase1-role") ?? "employee");

  useEffect(() => {
    localStorage.setItem("phase1-role", role);
  }, [role]);

  const rootRedirect = useMemo(() => defaultRouteByRole[role] ?? "/dashboard/employee", [role]);

  const allowed = useMemo(() => {
    return {
      employeeDashboard: menuItems.find((item) => item.to === "/dashboard/employee")?.roles ?? [],
      hrDashboard: menuItems.find((item) => item.to === "/dashboard/hr")?.roles ?? [],
      adminDashboard: menuItems.find((item) => item.to === "/dashboard/admin")?.roles ?? [],
      leaves: menuItems.find((item) => item.to === "/leaves")?.roles ?? [],
      approvals: menuItems.find((item) => item.to === "/approvals")?.roles ?? [],
      report: menuItems.find((item) => item.to === "/reports/leave-balance")?.roles ?? []
    };
  }, []);

  return (
    <Routes>
      <Route path="/login" element={<LoginPage role={role} onRoleChange={setRole} />} />
      <Route path="/" element={<AppShell role={role} onRoleChange={setRole} />}>
        <Route index element={<Navigate to={rootRedirect} replace />} />
        <Route
          path="dashboard/employee"
          element={
            <RoleGate role={role} allowedRoles={allowed.employeeDashboard}>
              <EmployeeDashboardPage />
            </RoleGate>
          }
        />
        <Route
          path="dashboard/hr"
          element={
            <RoleGate role={role} allowedRoles={allowed.hrDashboard}>
              <HrDashboardPage />
            </RoleGate>
          }
        />
        <Route
          path="dashboard/admin"
          element={
            <RoleGate role={role} allowedRoles={allowed.adminDashboard}>
              <AdminDashboardPage />
            </RoleGate>
          }
        />
        <Route
          path="leaves"
          element={
            <RoleGate role={role} allowedRoles={allowed.leaves}>
              <EmployeeLeavePage />
            </RoleGate>
          }
        />
        <Route
          path="approvals"
          element={
            <RoleGate role={role} allowedRoles={allowed.approvals}>
              <ApprovalsPage />
            </RoleGate>
          }
        />
        <Route
          path="reports/leave-balance"
          element={
            <RoleGate role={role} allowedRoles={allowed.report}>
              <LeaveBalanceReportPage />
            </RoleGate>
          }
        />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
