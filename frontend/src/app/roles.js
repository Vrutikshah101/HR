export function mapPrimaryRole(roles) {
  if (roles.some((x) => x.toLowerCase() === "admin")) {
    return "admin";
  }

  if (roles.some((x) => x.toLowerCase() === "hr")) {
    return "hr";
  }

  return "user";
}

export function defaultRouteByRole(role) {
  if (role === "admin") {
    return "/dashboard/admin";
  }

  if (role === "hr") {
    return "/dashboard/hr";
  }

  return "/dashboard/employee";
}

export function getMenuItems(roles) {
  const isAdmin = roles.some((x) => x.toLowerCase() === "admin");
  const isHr = roles.some((x) => x.toLowerCase() === "hr");
  const isUser = roles.some((x) => x.toLowerCase() === "user");

  const items = [];

  if (isUser || isHr || isAdmin) {
    items.push({ label: "Employee Dashboard", to: "/dashboard/employee", icon: "dashboard" });
    items.push({ label: "My Leaves", to: "/leaves", icon: "leaves" });
    items.push({ label: "Approvals", to: "/approvals", icon: "approvals" });
    items.push({ label: "Profile", to: "/profile", icon: "profile" });
  }

  if (isHr || isAdmin) {
    items.push({ label: "HR Dashboard", to: "/dashboard/hr", icon: "dashboard" });
    items.push({ label: "Leave Balance Report", to: "/reports/leave-balance", icon: "reports" });
  }

  if (isAdmin) {
    items.push({ label: "Admin Dashboard", to: "/dashboard/admin", icon: "dashboard" });
  }

  return items;
}
