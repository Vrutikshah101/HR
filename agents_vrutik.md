# agents_vrutik.md

## Project
Leave Management System (HR / Leave Workflow)

## Tech Stack
- Backend: ASP.NET Core Web API
- Frontend: React (Vite)
- Database: MySQL

## Working Principles Followed
- Phase-driven implementation
- Schema-first and contract-aligned development
- No hidden logic in UI/controllers
- Documentation + implementation sync
- Test-first validation on critical flows

## Completed Scope Summary

### Phase -1: Planning and Documentation
Completed and maintained mandatory docs under `docs/`:
- requirements.md
- domain-model.md
- schema.sql
- api-contract.md
- workflow.md
- access-matrix.md
- assumptions.md
- decision-log.md
- test-plan.md
- phase-plan.md

### Phase 0: Environment and Docker
- Added container setup for MySQL, backend, frontend.
- Added Dockerfiles and compose wiring.
- Added env variable support for connection string/API base URL.
- Also supported local non-Docker run path due machine constraints.

### Phase 1: Frontend Foundation (then upgraded to live API UX)
Initial:
- Layout shell, role-based routes, static dashboards/forms.

Upgraded:
- Real login via `/api/auth/login`.
- JWT parsing + role-based redirects.
- Protected routes and logout flow.
- API client bearer token interceptor.
- Unauthorized/session-expiry redirect handling.

### Phase 2: DB + Backend Foundation
- EF Core + Pomelo MySQL configured.
- `ApplicationDbContext` and entity mappings implemented.
- Initial migration created and applied.
- DI and infrastructure registration finalized.

### Phase 3: Auth + Users + Hierarchy
- JWT token service and auth pipeline implemented.
- Password hashing (PBKDF2) implemented.
- Users APIs implemented (`POST /api/users`, `GET /api/users`).
- Hierarchy APIs implemented (`PUT /api/hierarchy`, `GET /api/hierarchy/{employeeId}`).
- Dev seeding added (admin/hr/manager/employee).

### Phase 4: Leave Apply
- Leave apply validations and workflow entry implemented.
- Leave type/date/reason/days/overlap/balance validations.
- My leave history retrieval implemented.

### Phase 5: Approval Workflow
- Level 1 / Level 2 approval transitions implemented.
- Reject flow with mandatory comment validation.
- Audit trail persisted in approvals table.
- Approve/reject APIs wired and validated.

### Phase 6: Leave Balance + Holidays
- Working-day logic excluding weekends/holidays.
- Balance check at apply time.
- Deduction on final approval.
- Restoration on approved cancellation.
- APIs: holidays + my balances.

### Phase 7: Dashboards
- Role-based KPI service implemented (Employee/Manager/HR/Admin).
- Dashboard endpoints converted to DB-driven metrics.

### Phase 8: MIS Reports
- Reports service implemented with filters.
- CSV export mode added via `format=csv`.
- Endpoints for leave-balance, department-summary, monthly-utilization, approval-summary.

### Phase 9: Notifications + Hardening
- Notification abstraction added with logging channel baseline.
- Global exception middleware.
- Security headers.
- Rate limiting (global + auth policy).
- Password policy tightening.

### Phase 10: QA & Release Readiness
- Backend test project added.
- Tests for hashing, leave balance logic, workflow validation.
- Build + tests executed successfully.

## Post-Phase Enhancements Completed

### UX/UI Modernization
- Login page redesigned (multiple iterations).
- Inner workspace redesign with improved shell, cards, tables, visual hierarchy.
- Responsive behavior improvements.

### Navigation and Layout
- Breadcrumbs implemented.
- Top-right user identity area with avatar and sign-out.
- Footer added to inner workspace.
- Smart sidebar behavior (auto-hide/pin/mobile toggle).

### Data Grids
- Reusable searchable/sortable/filterable grid component introduced.
- Applied in leaves, approvals, and reports screens.

### Profile Module
- Backend profile APIs added:
  - `GET /api/users/me`
  - `PUT /api/users/me`
- Frontend profile page added for view/edit.

### Identity Display Changes
- UI shifted from raw IDs to user-facing info where available.
- Approval list now uses employee name/email in UI context.

### Activity Tracking
- Client-side activity tracking added:
  - page visits
  - login/logout
  - leave apply/cancel
  - approval actions
  - report filter/export
  - profile update
- Activity display added to Profile page (Recent Activity).
- Reliability update: event-based refresh + storage fallback.

### Icons and Branding
- App icon set introduced for navigation/actions.
- Favicon and apple-touch-icon added:
  - `frontend/public/favicon.svg`
  - `frontend/public/apple-touch-icon.svg`
- `index.html` updated with icon metadata.

## Current Runtime Notes
- For PowerShell policy restrictions, `npm.cmd` is used instead of `npm`.
- If backend is already running, some dotnet full-solution builds may lock binaries; alternate output build path can be used.

## Current Status
- Functional backend/API completed across planned phases.
- Frontend is integrated with live APIs for core flows.
- UI/UX substantially upgraded, with ongoing iterative polish based on feedback.
