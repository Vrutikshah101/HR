# Test Plan (Up To Phase 10)

## Automated Validation
- Command: `dotnet build backend/LeaveManagement.sln`
- Result: Passed
- Command: `dotnet test backend/LeaveManagement.sln`
- Result: Passed (3 tests)

## Phase 6 (Leave Balance + Holidays)
- Verified endpoints:
  - `GET /api/leaves/holidays`
  - `GET /api/leaves/my-balances`
- Verified rules:
  - working-day checks exclude weekends/holidays
  - apply requires sufficient balance
  - final approval deducts balance
  - approved cancellation restores balance

## Phase 7 (Dashboards)
- Verified role dashboard endpoints are DB-driven:
  - employee
  - manager
  - hr
  - admin

## Phase 8 (MIS Reports)
- Verified JSON and filtered report responses.
- Verified CSV export through `format=csv`.

## Phase 9 (Notifications + Hardening)
- Verified global exception middleware returns problem details payload.
- Verified rate limiter policies:
  - global fixed-window limiter
  - auth login specific limiter
- Verified security headers middleware is active.
- Verified notification hooks are emitted through logging service.

## Phase 10 (QA & Release)
- Added backend test project and integrated into solution.
- Added unit tests:
  - password hash/verify
  - working-day holiday/weekend exclusion
  - reject comment requirement
- Release readiness items documented in phase plan and journey.
