# API Contract (Current Scaffold + Phase 5)

## Auth

### POST `/api/auth/login`
- Confirmed request: `email`, `password`.
- Confirmed response: `accessToken`, `refreshToken`.
- Confirmed behavior: validates user existence, active status, and password hash.

## Users

### POST `/api/users`
### GET `/api/users`
- Authorization: `Admin` or `Hr` role required.

## Hierarchy

### PUT `/api/hierarchy`
### GET `/api/hierarchy/{employeeId}`
- Authorization: `Admin` or `Hr` role required.

## Leaves

### GET `/api/leaves/types`
- Authorization: authenticated users.
- Response: list of leave type code/name values from `Leave:Types` config.

### POST `/api/leaves`
- Authorization: authenticated users.
- DB-backed create with validation + overlap checks.

### GET `/api/leaves/my`
- Authorization: authenticated users.
- Returns own persisted leave history.

### GET `/api/leaves/team-pending`
- Authorization: authenticated users.
- Returns pending requests for configured Level1/Level2 approver scope.

### GET `/api/leaves/{id}`
- Authorization: owner, configured approver, or `Admin/Hr`.

### POST `/api/leaves/{id}/approve`
- Authorization: authenticated users.
- Workflow rules:
  - request must be in pending status
  - current user must match configured approver for current level
  - `PendingLevel1` approval moves to `PendingLevel2` when Level2 exists, else `Approved`
  - `PendingLevel2` approval moves to `Approved`
- Side effect: inserts audit row in `leave_request_approvals`.

### POST `/api/leaves/{id}/reject`
- Authorization: authenticated users.
- Workflow rules:
  - request must be in pending status
  - current user must match configured approver for current level
  - reject comment is mandatory
  - status moves to `Rejected`
- Side effect: inserts audit row in `leave_request_approvals`.

### POST `/api/leaves/{id}/cancel`
- Authorization: authenticated users.
- Only own pending requests can be cancelled.

## Dashboard
- Authorization remains role-scoped (`User/Hr/Admin` as configured).

## Reports
- Authorization remains `Hr/Admin`.
