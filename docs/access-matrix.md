# Access Matrix

| Role | Login | Apply Leave | View Own Leaves | Approve Team Leaves | Manage Users | Manage Hierarchy | View HR Dashboard | View Admin Dashboard | View Reports |
|---|---|---|---|---|---|---|---|---|---|
| Employee/User | Allowed | Allowed | Allowed | Allowed (if configured approver, workflow phase pending) | Not Allowed | Not Allowed | Not Allowed | Not Allowed | Not Allowed |
| HR | Allowed | Allowed | Allowed | Allowed (org policy scope) | Allowed | Allowed | Allowed | Not Allowed | Allowed |
| Admin | Allowed | Allowed | Allowed | Allowed (org-wide) | Allowed | Allowed | Allowed | Allowed | Allowed |

## Approval Authorization Rules
- Confirmed: action allowed only for configured level approver.
- Confirmed: action allowed only at correct request status.
- Confirmed: employee visibility is own data only.
- Confirmed: approver visibility is subordinate data only.
- Assumption: final HR scope breadth may be org-wide and is pending implementation detail.
