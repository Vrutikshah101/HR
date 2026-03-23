# Workflow

## Leave Apply to Approval

1. Employee submits leave request.
2. Service validates leave type/date span/days/reason and overlap.
3. Request enters pending state based on hierarchy (`PendingLevel1` or `PendingLevel2`).
4. Level1 approver can approve/reject only when request is `PendingLevel1`.
5. Level2 approver can approve/reject only when request is `PendingLevel2`.
6. Approve transitions:
   - Level1 approve -> `PendingLevel2` if Level2 exists; otherwise `Approved`.
   - Level2 approve -> `Approved`.
7. Reject transitions:
   - Level1 or Level2 reject -> `Rejected` (comment required).
8. Every approve/reject action inserts `leave_request_approvals` audit entry.
9. Employee can cancel own pending request -> `Cancelled`.

## Guardrails
- Approval allowed only for configured level approver and correct status.
- Unauthorized approver action is rejected.
- Non-pending requests cannot be approved/rejected.
- No hierarchy/approver configuration blocks apply/approval actions as appropriate.
