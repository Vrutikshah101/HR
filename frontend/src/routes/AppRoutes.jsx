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
import { AnalyticsDashboardPage } from "../features/reports/pages/AnalyticsDashboardPage";
import { DepartmentsMasterPage } from "../features/masters/pages/DepartmentsMasterPage";
import { DesignationsMasterPage } from "../features/masters/pages/DesignationsMasterPage";
import { DepartmentDesignationMapPage } from "../features/masters/pages/DepartmentDesignationMapPage";
import { LeaveTypesMasterPage } from "../features/masters/pages/LeaveTypesMasterPage";
import { HolidaysMasterPage } from "../features/masters/pages/HolidaysMasterPage";
import { UserRegistrationPage } from "../features/users/pages/UserRegistrationPage";
import { ActivityTrackerPage } from "../features/activity/pages/ActivityTrackerPage";
import { HierarchySetupPage } from "../features/hierarchy/pages/HierarchySetupPage";
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
        <Route path="activity" element={<ActivityTrackerPage />} />
        <Route
          path="users/new"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <UserRegistrationPage />
            </RoleRoute>
          }
        />
        <Route
          path="hierarchy/setup"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <HierarchySetupPage />
            </RoleRoute>
          }
        />
        <Route
          path="masters/departments"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <DepartmentsMasterPage />
            </RoleRoute>
          }
        />
        <Route
          path="masters/designations"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <DesignationsMasterPage />
            </RoleRoute>
          }
        />
        <Route
          path="masters/department-designation-maps"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <DepartmentDesignationMapPage />
            </RoleRoute>
          }
        />
        <Route
          path="masters/leave-types"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <LeaveTypesMasterPage />
            </RoleRoute>
          }
        />
        <Route
          path="masters/holidays"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <HolidaysMasterPage />
            </RoleRoute>
          }
        />

        <Route
          path="reports/leave-balance"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <LeaveBalanceReportPage />
            </RoleRoute>
          }
        />
        <Route
          path="reports/analytics"
          element={
            <RoleRoute roles={roles} allow={["Hr", "Admin"]}>
              <AnalyticsDashboardPage />
            </RoleRoute>
          }
        />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
