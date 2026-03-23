# Domain Model

## Confirmed Entities

### User
- Confirmed fields: `Id`, `Email`, `PasswordHash`, `IsActive`, `CreatedAtUtc`.

### UserRoleAssignment
- Confirmed fields: `UserId`, `RoleCode`.
- Confirmed purpose: maps users to one or more roles.

### Employee
- Confirmed fields: `Id`, `UserId`, `EmployeeCode`, `FullName`, `Department`, `Designation`.

### ReportingHierarchy
- Confirmed fields: `Id`, `EmployeeId`, `Level1ApproverEmployeeId`, `Level2ApproverEmployeeId`.
- Confirmed behavior intent: supports 1-level (Level2 null) or 2-level approval.

### LeaveRequest
- Confirmed fields: `Id`, `EmployeeId`, `LeaveTypeCode`, `StartDate`, `EndDate`, `Days`, `Reason`, `Status`, `CreatedAtUtc`.
- Confirmed statuses from enum:
  - `Draft`
  - `PendingLevel1`
  - `PendingLevel2`
  - `Approved`
  - `Rejected`
  - `Cancelled`

### LeaveRequestApproval
- Confirmed fields: `Id`, `LeaveRequestId`, `ApprovalLevel`, `ApproverEmployeeId`, `Action`, `Comment`, `ActionedAtUtc`.

### LeaveBalance
- Confirmed fields: `Id`, `EmployeeId`, `LeaveTypeCode`, `OpeningBalance`, `Used`, `Adjustments`.
- Confirmed computed property: `Available = OpeningBalance + Adjustments - Used`.

## Relationship Summary
- Confirmed: `UserRoleAssignment.UserId -> User.Id` (many roles per user).
- Confirmed: `Employee.UserId -> User.Id` (1:1 logical mapping).
- Confirmed: `ReportingHierarchy.EmployeeId -> Employee.Id`.
- Confirmed: `ReportingHierarchy.Level1ApproverEmployeeId/Level2ApproverEmployeeId -> Employee.Id`.
- Confirmed: `LeaveRequest.EmployeeId -> Employee.Id`.
- Confirmed: `LeaveRequestApproval.LeaveRequestId -> LeaveRequest.Id`.
- Confirmed: `LeaveRequestApproval.ApproverEmployeeId -> Employee.Id`.
- Confirmed: `LeaveBalance.EmployeeId -> Employee.Id`.

## Open Questions
- Open Question: Leave type master table is not yet defined; currently represented as `LeaveTypeCode` string.
- Open Question: Holiday calendar entity is not defined in current domain.
- Open Question: Audit log entity for non-approval actions is not defined yet.
