# Decision Log

| ID | Date | Decision | Type | Rationale | Follow-up |
|---|---|---|---|---|---|
| DEC-001 | 2026-03-23 | Use domain-backed docs and traceability-first execution. | Confirmed | Reduces hallucination and improves auditability. | Keep docs in sync with code. |
| DEC-002 | 2026-03-23 | Keep leave types and holidays config-driven until master tables are introduced. | Assumption | Delivers functionality without new schema invention. | Add normalized master data tables in future enhancement. |
| DEC-003 | 2026-03-23 | Use log-based notification adapter for Phase 9 baseline. | Confirmed | Enables notification hooks without external service dependency. | Replace with email/SMS/in-app delivery provider later. |
| DEC-004 | 2026-03-23 | Add global exception middleware + rate limiting for hardening baseline. | Confirmed | Improves resilience and abuse protection. | Fine-tune limits and response policy in production rollout. |
| DEC-005 | 2026-03-23 | Add automated backend tests and include test project in solution for release readiness. | Confirmed | Establishes repeatable QA gate in repository. | Expand coverage with integration/API tests in subsequent iteration. |
