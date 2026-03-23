# Workflow

## Leave Apply to Approval to Balance

1. Employee applies leave.
2. System validates:
   - leave type exists
   - start/end range
   - reason
   - days > 0
   - no overlap with active leave
   - days within working-day count (excluding weekends/holidays)
   - sufficient leave balance
3. System sets pending status based on hierarchy (`PendingLevel1` or `PendingLevel2`).
4. Approver acts at valid level only.
5. Approval transitions:
   - L1 -> L2 pending (if L2 exists) or Approved
   - L2 -> Approved
6. Rejection transitions to Rejected (comment required).
7. Approval/rejection audit row is recorded.
8. On final approval, leave balance is deducted.
9. On cancellation of approved leave, balance is restored.

## Dashboard and MIS Flow
- Dashboard cards are DB-calculated per role.
- MIS endpoints support filters and CSV export mode.
