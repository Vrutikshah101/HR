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
    items.push({
      label: "Leave Master",
      icon: "leaves",
      children: [
        { label: "My Leave", to: "/leaves", icon: "leaves" },
        { label: "Approval", to: "/approvals", icon: "approvals" },
        { label: "Activity Tracker", to: "/activity", icon: "reports" }
      ]
    });
  }

  if (isHr || isAdmin) {
    items.push({
      label: "Organization Setup",
      icon: "profile",
      children: [
        { label: "User Registration", to: "/users/new", icon: "profile" },
        { label: "Hierarchy Workflow", to: "/hierarchy/setup", icon: "reports" }
      ]
    });

    const reportChildren = [
      { label: "Leave Balance Report", to: "/reports/leave-balance", icon: "reports" }
    ];

    if (isAdmin) {
      reportChildren.push({ label: "Admin Dashboards", to: "/dashboard/admin", icon: "dashboard" });
    } else if (isHr) {
      reportChildren.push({ label: "Admin Dashboards", to: "/dashboard/hr", icon: "dashboard" });
    }

    items.push({
      label: "Reports",
      icon: "reports",
      children: reportChildren
    });
  }

  return items;
}
