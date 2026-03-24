function baseProps(props) {
  return {
    viewBox: "0 0 24 24",
    fill: "none",
    stroke: "currentColor",
    strokeWidth: 1.8,
    strokeLinecap: "round",
    strokeLinejoin: "round",
    ...props
  };
}

export function DashboardIcon(props) {
  return (
    <svg {...baseProps(props)}>
      <rect x="3" y="3" width="7" height="7" rx="1.5" />
      <rect x="14" y="3" width="7" height="4" rx="1.5" />
      <rect x="14" y="10" width="7" height="11" rx="1.5" />
      <rect x="3" y="13" width="7" height="8" rx="1.5" />
    </svg>
  );
}

export function LeavesIcon(props) {
  return (
    <svg {...baseProps(props)}>
      <path d="M5 12c0-5 4.5-8.5 11-9-1 6.5-4 11-9 11H5Z" />
      <path d="M5 12c1.5 5 5 8 11 9-1.5-6-5-9-11-9Z" />
    </svg>
  );
}

export function ApprovalIcon(props) {
  return (
    <svg {...baseProps(props)}>
      <rect x="3" y="4" width="18" height="16" rx="2" />
      <path d="m8 12 2.3 2.3L16 9" />
    </svg>
  );
}

export function ReportIcon(props) {
  return (
    <svg {...baseProps(props)}>
      <path d="M7 3h7l5 5v13H7z" />
      <path d="M14 3v5h5" />
      <path d="M10 14h6M10 18h6M10 10h3" />
    </svg>
  );
}

export function UserIcon(props) {
  return (
    <svg {...baseProps(props)}>
      <circle cx="12" cy="8" r="3.2" />
      <path d="M5 20a7 7 0 0 1 14 0" />
    </svg>
  );
}

export function LogoutIcon(props) {
  return (
    <svg {...baseProps(props)}>
      <path d="M10 17v2a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h3a2 2 0 0 1 2 2v2" />
      <path d="M21 12h-8" />
      <path d="m18 9 3 3-3 3" />
    </svg>
  );
}
