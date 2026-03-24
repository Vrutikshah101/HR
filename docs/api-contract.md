# API Contract (Current Scaffold + Phase 8)

## Auth
- `POST /api/auth/login`

## Users
- `POST /api/users`
- `GET /api/users`
- `GET /api/users/me`
- `PUT /api/users/me`

## Hierarchy
- `PUT /api/hierarchy`
- `GET /api/hierarchy/{employeeId}`

## Leaves
- `GET /api/leaves/types`
- `GET /api/leaves/holidays`
- `GET /api/leaves/my-balances`
- `POST /api/leaves`
- `GET /api/leaves/my`
- `GET /api/leaves/team-pending`
- `GET /api/leaves/{id}`
- `POST /api/leaves/{id}/approve`
- `POST /api/leaves/{id}/reject`
- `POST /api/leaves/{id}/cancel`

### Leave Behavior Notes
- Apply validates type/date/reason/days and prevents overlap.
- Working-day validation excludes weekends and configured holidays.
- Sufficient leave balance is required at apply time.
- Final approval deducts leave balance.
- Cancelling approved leave restores deducted balance.
- Approve/reject writes audit records in `leave_request_approvals`.

## Dashboard (DB-driven)
- `GET /api/dashboard/employee`
- `GET /api/dashboard/manager`
- `GET /api/dashboard/hr`
- `GET /api/dashboard/admin`

### Dashboard Metric Sources (Current)
- Employee: `leave_balances`, `leave_requests`
- Manager: `reporting_hierarchies`, `leave_requests`
- HR: `leave_requests`
- Admin: `users`, `employees`, `leave_request_approvals`

## Reports (MIS)
- `GET /api/reports/leave-balance`
- `GET /api/reports/department-summary`
- `GET /api/reports/monthly-utilization`
- `GET /api/reports/approval-summary`

## Masters (Schema Alignment Phase 1)
- `GET /api/masters/departments`
- `POST /api/masters/departments`
- `GET /api/masters/designations`
- `POST /api/masters/designations`
- `GET /api/masters/leave-types`
- `POST /api/masters/leave-types`
- `GET /api/masters/holidays`
- `POST /api/masters/holidays`

### Report Filters
- Leave balance: `department`, `leaveTypeCode`, `format`
- Department summary: `dateFrom`, `dateTo`, `format`
- Monthly utilization: `year`, `format`
- Approval summary: `dateFrom`, `dateTo`, `format`

### Export
- `format=csv` returns downloadable CSV file.
- Default response is JSON rows.
