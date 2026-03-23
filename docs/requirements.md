# Requirements (Phase -1 to Phase 3 Baseline)

## Scope Status
- Confirmed: This baseline covers planning artifacts through Phase 3 using implemented entities and API scaffolding.
- Assumption: Existing leave/approval endpoints remain scaffold-level until Phase 4/5.
- Open Question: Exact business SLA rules (cutoff times, notice period, carry-forward) are not defined yet.

## Functional Requirements

| ID | Requirement | Type |
|---|---|---|
| REQ-001 | User can log in and receive token response. | Confirmed |
| REQ-002 | Employee can submit leave request with type, date range, days, reason. | Confirmed |
| REQ-003 | Employee can view own leave requests. | Confirmed |
| REQ-004 | Approver can view team pending requests. | Confirmed |
| REQ-005 | Approver can approve/reject at assigned level. | Confirmed |
| REQ-006 | Employee can cancel leave request. | Confirmed |
| REQ-007 | Employee/HR/Admin dashboards expose static KPI cards by role endpoint. | Confirmed |
| REQ-008 | Leave balance report endpoint returns report rows. | Confirmed |
| REQ-009 | Approval supports 1-level or 2-level hierarchy. | Confirmed |
| REQ-010 | Role-based UI navigation is visible per role in mocked mode for Phase 1. | Confirmed |
| REQ-011 | Admin/HR can create users with roles and employee profile mapping. | Confirmed |
| REQ-012 | Admin/HR can upsert and retrieve employee reporting hierarchy. | Confirmed |
| REQ-013 | Role-based API authorization is enforced via JWT claims. | Confirmed |

## Non-Functional Requirements
- Confirmed: API uses ASP.NET Core and Swagger in development.
- Confirmed: Frontend uses React + Vite.
- Confirmed: Stack must be container runnable via single docker compose command in Phase 0.
- Open Question: Performance targets and audit retention period are not yet defined.

## Traceability Matrix

| Requirement | API | DB | UI | Test |
|---|---|---|---|---|
| REQ-001 | `POST /api/auth/login` | `users`, `user_roles` | `/login` | API login success/failure test |
| REQ-002 | `POST /api/leaves` | `leave_requests` | `/leaves` apply form | Validation + creation integration test |
| REQ-003 | `GET /api/leaves/my` | `leave_requests` | `/leaves` history grid | Filter-by-current-user API test |
| REQ-004 | `GET /api/leaves/team-pending` | `leave_requests`, `reporting_hierarchies` | `/approvals` queue | Approver visibility test |
| REQ-005 | `POST /api/leaves/{id}/approve`, `POST /api/leaves/{id}/reject` | `leave_requests`, `leave_request_approvals`, `reporting_hierarchies` | `/approvals` action form | Level/role authorization tests |
| REQ-006 | `POST /api/leaves/{id}/cancel` | `leave_requests` | `/leaves` cancel action | Status transition test |
| REQ-007 | `GET /api/dashboard/employee|manager|hr|admin` | derived from leave/balance/hierarchy tables | `/dashboard/*` | KPI correctness test |
| REQ-008 | `GET /api/reports/leave-balance` | `leave_balances`, `employees` | `/reports/leave-balance` | report row accuracy test |
| REQ-009 | hierarchy APIs + approval endpoints | `reporting_hierarchies` | `/approvals` | 1-level vs 2-level workflow test |
| REQ-010 | N/A (UI mocked) | N/A | role switcher + menu visibility | UI role visibility unit test |
| REQ-011 | `POST /api/users` | `users`, `user_roles`, `employees` | admin/hr management screen (future) | duplicate + role validation tests |
| REQ-012 | `PUT /api/hierarchy`, `GET /api/hierarchy/{employeeId}` | `reporting_hierarchies` | admin/hr management screen (future) | hierarchy validation tests |
| REQ-013 | `[Authorize]` protected endpoints | `users`, `user_roles` | UI token flow (future integration) | unauthorized/forbidden API tests |
