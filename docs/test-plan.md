# Test Plan (Up To Phase 5)

## Phase 5 (Approval Workflow)
- Confirmed execution: backend build passed after workflow service integration.
- API scenarios to validate:
  - Level1 approver approves `PendingLevel1` and request moves to `PendingLevel2` or `Approved`.
  - Level2 approver approves `PendingLevel2` and request moves to `Approved`.
  - Reject requires non-empty comment and sets status `Rejected`.
  - Approver mismatch returns `400` with validation message.
  - Non-pending request approve/reject returns `400`.
  - Audit entry inserted into `leave_request_approvals` for each approve/reject action.

## Existing Phase Coverage
- Phase 0-4 checks remain as previously documented.
