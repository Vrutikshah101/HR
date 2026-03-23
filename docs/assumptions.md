# Assumptions Log

| ID | Item | Label | Impact | Mitigation |
|---|---|---|---|---|
| ASM-001 | Dashboard KPIs in current controllers are scaffold placeholders. | Confirmed | Values not production-accurate. | Replace with DB-derived formulas in Phase 7. |
| ASM-002 | Refresh token persistence/rotation is not implemented yet. | Confirmed | Login response includes placeholder refresh token value. | Implement refresh-token store and revoke flow in hardening phase. |
| ASM-003 | Cancellation policy after approval stage is undefined. | Open Question | Could create workflow ambiguity. | Add decision before Phase 6. |
| ASM-004 | Holiday entity and policy are undefined in current schema. | Open Question | Blocks full balance calculations. | Define in Phase 6 design update. |
| ASM-005 | Leave types are configuration-driven (`Leave:Types`) because no leave type master table exists yet. | Assumption | Dynamic leave type management is limited. | Add leave type master table and migration in later phase. |
| ASM-006 | Development seed credentials are for local/test only and not production-safe. | Confirmed | Security risk if reused outside dev. | Document and disable in production deployment. |
