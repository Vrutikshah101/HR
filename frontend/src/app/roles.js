export const roleOptions = [
  { value: "employee", label: "Employee" },
  { value: "approver", label: "Approver" },
  { value: "hr", label: "HR" },
  { value: "admin", label: "Admin" }
];

export const defaultRouteByRole = {
  employee: "/dashboard/employee",
  approver: "/approvals",
  hr: "/dashboard/hr",
  admin: "/dashboard/admin"
};

export const menuItems = [
  {
    label: "Employee Dashboard",
    to: "/dashboard/employee",
    roles: ["employee", "approver"]
  },
  {
    label: "HR Dashboard",
    to: "/dashboard/hr",
    roles: ["hr", "admin"]
  },
  {
    label: "Admin Dashboard",
    to: "/dashboard/admin",
    roles: ["admin"]
  },
  {
    label: "My Leaves",
    to: "/leaves",
    roles: ["employee", "approver", "hr", "admin"]
  },
  {
    label: "Approvals",
    to: "/approvals",
    roles: ["approver", "hr", "admin"]
  },
  {
    label: "Leave Balance Report",
    to: "/reports/leave-balance",
    roles: ["hr", "admin"]
  }
];
