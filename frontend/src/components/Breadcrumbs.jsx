import { Link, useLocation } from "react-router-dom";

const labelMap = {
  dashboard: "Dashboard",
  employee: "Employee",
  hr: "HR",
  admin: "Admin",
  leaves: "My Leaves",
  approvals: "Approvals",
  activity: "Activity Tracker",
  hierarchy: "Hierarchy",
  setup: "Workflow Setup",
  reports: "Reports",
  "leave-balance": "Leave Balance",
  masters: "Masters",
  departments: "Departments",
  designations: "Designations",
  "department-designation-maps": "Dept-Desig Mapping",
  "leave-types": "Leave Types",
  holidays: "Holidays",
  users: "Users",
  new: "Register"
};

function toLabel(segment) {
  return labelMap[segment] ?? segment.replace(/-/g, " ").replace(/\b\w/g, (x) => x.toUpperCase());
}

export function Breadcrumbs() {
  const location = useLocation();
  const parts = location.pathname.split("/").filter(Boolean);

  if (parts.length === 0) {
    return null;
  }

  return (
    <nav className="breadcrumbs" aria-label="Breadcrumb">
      <Link to="/">Home</Link>
      {parts.map((part, index) => {
        const isLast = index === parts.length - 1;
        const to = `/${parts.slice(0, index + 1).join("/")}`;
        return (
          <span key={to} className="crumb-part">
            <span className="crumb-sep">/</span>
            {isLast ? <span>{toLabel(part)}</span> : <Link to={to}>{toLabel(part)}</Link>}
          </span>
        );
      })}
    </nav>
  );
}
