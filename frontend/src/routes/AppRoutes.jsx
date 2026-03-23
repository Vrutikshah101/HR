import { Navigate, Route, Routes } from "react-router-dom";
import { useMemo, useState } from "react";
import { AppShell } from "../components/AppShell";
import { UnauthorizedPage } from "../components/UnauthorizedPage";
import { LoginPage } from "../features/auth/pages/LoginPage";
import { EmployeeLeavePage } from "../features/leaves/pages/EmployeeLeavePage";
import { ApprovalsPage } from "../features/approvals/pages/ApprovalsPage";
import { EmployeeDashboardPage } from "../features/dashboard/pages/EmployeeDashboardPage";
import { HrDashboardPage } from "../features/dashboard/pages/HrDashboardPage";
import { AdminDashboardPage } from "../features/dashboard/pages/AdminDashboardPage";
import { LeaveBalanceReportPage } from "../features/reports/pages/LeaveBalanceReportPage";
import { clearToken, getToken } from "../services/tokenStorage";
import { getRolesFromToken, hasRole } from "../services/jwt";
import { defaultRouteByRole, mapPrimaryRole } from "../app/roles";

function ProtectedRoute({ token, children }) {
  if (!token) {
    return <Navigate to="/login" replace />;
  }

  return children;
}

function RoleRoute({ roles, allow, children }) {
  const ok = allow.some((role) => hasRole(roles, role));
  if (!ok) {
    return <UnauthorizedPage role={mapPrimaryRole(roles)} />;
  }

  return children;
}

export function AppRoutes() {
  const [token, setToken] = useState(() => getToken());
  const [roles, setRoles] = useState(() => getRolesFromToken(getToken()));

  const rootRedirect = useMemo(() => defaultRouteByRole(mapPrimaryRole(roles)), [roles]);

  function handleLoginSuccess(newRoles) {
    setToken(getToken());
    setRoles(newRoles);
  }

  function handleLogout() {
    clearToken();
    setToken(null);
    setRoles([]);
  }

  return (
    <Routes>
      <Route
        path="/login"
        element={token ? <Navigate to={rootRedirect} replace /> : <LoginPage onLoginSuccess={handleLoginSuccess} />}
      />

      <Route
        path="/"
        element={
          <ProtectedRoute token={token}>
            <AppShell roles={roles} onLogout={handleLogout} />
          </ProtectedRoute>
        }
      >
        <Route index element={<Navigate to={rootRedirect} replace />} />

        <Route
          path="dashboard/employee"
          element={
            <RoleRoute roles={roles} allow={["User", "Hr", "Admin"]}>
              <EmployeeDashboardPage />
            </RoleRoute>
          }
        />

        <Route
          path="dashboard/hr"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <HrDashboardPage />
            </RoleRoute>
          }
        />

        <Route
          path="dashboard/admin"
          element={
            <RoleRoute roles={roles} allow={["Admin"]}>
              <AdminDashboardPage />
            </RoleRoute>
          }
        />

        <Route path="leaves" element={<EmployeeLeavePage />} />
        <Route path="approvals" element={<ApprovalsPage />} />

        <Route
          path="reports/leave-balance"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <LeaveBalanceReportPage />
            </RoleRoute>
          }
        />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
