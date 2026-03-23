# Decision Log

| ID | Date | Decision | Type | Rationale | Follow-up |
|---|---|---|---|---|---|
| DEC-001 | 2026-03-23 | Use current domain entities as authoritative baseline for docs. | Confirmed | Prevents undocumented entity invention. | Keep reconciling docs with implementation. |
| DEC-002 | 2026-03-23 | Add Docker Compose with backend/frontend/mysql. | Confirmed | Meets single-command setup target. | Validate runtime health in CI. |
| DEC-003 | 2026-03-23 | Keep Phase 1 UI mock-data only. | Confirmed | Avoids backend leakage into UI phase. | Replace with real integration later. |
| DEC-004 | 2026-03-23 | Use explicit MySQL server version `8.4.0` in EF config. | Confirmed | Enables design-time migration generation offline. | Revisit if runtime MySQL version changes. |
| DEC-005 | 2026-03-23 | Use PBKDF2 hashing and JWT role claims for auth baseline. | Confirmed | Secure practical baseline for Phase 3. | Add refresh token persistence later. |
| DEC-006 | 2026-03-23 | Execute migrations and dev seeding at startup in development flow. | Confirmed | Speeds local testing and onboarding. | Gate/disable for production profile. |
| DEC-007 | 2026-03-23 | Implement leave apply/history/cancel logic in dedicated service layer (`ILeaveRequestService`). | Confirmed | Keeps business rules out of controllers. | Expand service for full approve/reject workflow in Phase 5. |
| DEC-008 | 2026-03-23 | Source leave types from configuration (`Leave:Types`) until master table exists. | Assumption | Delivers Phase 4 leave type support without schema invention. | Introduce leave type master schema in a future phase. |
