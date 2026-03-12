# AGENTS.md

## Project Name
Leave Management System

## Project Overview
This project is a **Leave Management System** built with:

- **Backend:** ASP.NET Core Web API
- **Frontend:** React.js
- **Database:** MySQL

The system supports the following primary user types:

- **Admin**
- **HR**
- **User / Employee**
- **Manager-like approver behavior** for any employee who has subordinates

A user with the normal `User` role may still act as an approver for subordinate employees based on reporting hierarchy.  
The leave approval workflow supports **up to 2 levels**.

The product must provide:

- Leave application and approval workflow
- Employee hierarchy-based approvals
- Leave balance tracking
- Role-based dashboards
- MIS reports
- Audit trail
- Secure authentication and authorization

---

## Product Goals

1. Build a clean, scalable Leave Management System.
2. Support **Admin, HR, and Employee** roles.
3. Support **hierarchy-based leave approval** where employees can approve subordinate leave.
4. Allow **1-level or 2-level configurable approval flow**.
5. Provide rich **MIS reports** and **user-specific dashboards**.
6. Maintain clean code, clear separation of concerns, and production-ready structure.

---

## Business Rules

### Roles
#### Admin
- Full access to system configuration and data
- Manage users, roles, departments, designations, leave types, holidays, policies
- View all leave requests and reports

#### HR
- Manage employees and leave operations
- View and process leave requests
- Maintain leave balances and reports
- Can act as final approver when policy requires

#### User / Employee
- Apply for leave
- View own leave balance and history
- Cancel own leave within allowed policy
- Approve/reject leave of subordinates if assigned in hierarchy

---

## Approval Workflow Rules

1. Every employee may have:
   - `Level1Approver`
   - `Level2Approver`

2. Approval flow can be:
   - **Single-level**
   - **Two-level**

3. Request states:
   - `Draft`
   - `PendingLevel1`
   - `PendingLevel2`
   - `Approved`
   - `Rejected`
   - `Cancelled`

4. Workflow:
   - Employee submits leave
   - Request goes to Level 1 approver
   - If Level 1 approves:
     - if Level 2 exists, move to Level 2
     - otherwise mark Approved
   - If any approver rejects, mark Rejected
   - Leave balance is deducted only on final approval
   - Approved leave cancellation should restore balance as per policy

5. A normal user can approve leaves **only for employees under their reporting hierarchy**.

---

## Functional Modules

### 1. Authentication and Authorization
- Login
- JWT-based authentication
- Role-based authorization
- Policy-based and hierarchy-based access checks

### 2. Organization Management
- Users
- Employees
- Departments
- Designations
- Roles
- Reporting hierarchy

### 3. Leave Management
- Leave types
- Leave policies
- Apply leave
- Leave validation
- Leave history
- Cancellation

### 4. Approval Management
- Pending approvals
- Approval history
- Approve/reject actions
- Approval audit trail

### 5. Leave Balance
- Opening balance
- Accrual
- Usage
- Adjustments
- Closing balance

### 6. Dashboard
- Employee dashboard
- Manager dashboard
- HR dashboard
- Admin dashboard

### 7. MIS / Reports
- Employee leave ledger
- Department leave summary
- Manager approval report
- Monthly leave utilization
- Leave balance report
- Absence trend report
- Exception report

### 8. Notifications
- Email/in-app notifications for:
  - leave submitted
  - leave approved
  - leave rejected
  - leave cancelled

### 9. Audit and Logs
- Action logs
- Approval trail
- Changes to leave balances
- User activity logs

---

## Non-Functional Requirements

- Clean and modular architecture
- Secure APIs
- Responsive frontend
- Easy maintainability
- Good performance for MIS reports
- Clear validation and error handling
- Auditability for every approval action

---

## Recommended Architecture

### Backend
Use **Clean Architecture** or strong layered architecture.

Suggested backend projects:
- `LeaveManagement.API`
- `LeaveManagement.Application`
- `LeaveManagement.Domain`
- `LeaveManagement.Infrastructure`

### Frontend
Use React app with feature-based structure.

Suggested frontend folders:
- `src/features/auth`
- `src/features/users`
- `src/features/leaves`
- `src/features/approvals`
- `src/features/dashboard`
- `src/features/reports`
- `src/components`
- `src/services`
- `src/routes`
- `src/utils`

---

## Tech Stack

### Backend
- ASP.NET Core 8 Web API
- Entity Framework Core
- MySQL
- JWT Authentication
- FluentValidation
- AutoMapper
- Serilog
- Swagger / OpenAPI

### Frontend
- React.js
- Vite
- React Router
- Redux Toolkit or Zustand
- Axios
- Material UI or Ant Design
- Recharts or Chart.js
- React Hook Form

### Database
- MySQL 8+

---

## Core Entities

### User
Represents application user credentials and identity.

### Employee
Represents employee profile and business identity.

### Role
Represents system role:
- Admin
- HR
- User

### Department
Department master.

### Designation
Designation master.

### LeaveType
Examples:
- Casual Leave
- Sick Leave
- Earned Leave
- Loss of Pay

### LeavePolicy
Defines rules for each leave type.

### LeaveBalance
Stores per-user leave balance.

### LeaveRequest
Stores leave application data.

### LeaveRequestApproval
Stores approval step actions.

### ReportingHierarchy
Stores approver relationships:
- Employee
- Level1Approver
- Level2Approver

### Holiday
Stores holiday calendar.

### Notification
Stores system notifications.

### AuditLog
Stores audit trail.

---

## Suggested Database Tables

- `users`
- `roles`
- `user_roles`
- `employees`
- `departments`
- `designations`
- `reporting_hierarchy`
- `leave_types`
- `leave_policies`
- `leave_requests`
- `leave_request_approvals`
- `leave_balances`
- `leave_balance_transactions`
- `holidays`
- `notifications`
- `audit_logs`

---

## Dashboard Expectations

### Employee Dashboard
- Available leave balance
- Pending requests
- Recent leave history
- Team on leave today
- Upcoming holidays

### Manager Dashboard
- Pending approvals
- Team members on leave today
- Upcoming team leave
- Approval turnaround metrics
- Team leave summary

### HR Dashboard
- Total employees on leave today
- Pending approvals by level
- Approved/rejected this month
- Department-wise leave usage
- Leave exception snapshot

### Admin Dashboard
- Organization leave statistics
- Monthly trends
- Pending requests
- Role/user distribution
- Department-wise summary
- System-level metrics

---

## MIS Expectations

Reports must support filters such as:
- Date range
- Department
- Employee
- Leave type
- Status
- Approver

Reports should support export:
- Excel
- CSV
- PDF

Priority reports:
1. Employee leave ledger
2. Leave balance report
3. Department leave summary
4. Monthly leave utilization
5. Manager approval report
6. Exception report

---

## Access Control Rules

### General
- Users may access only allowed resources.
- Employees may view only their own records.
- Managers/approvers may view subordinate leave requests.
- HR and Admin may view broader datasets based on role.

### Hierarchy Rule
A user can approve leave if and only if:
- they are configured as `Level1Approver` or `Level2Approver`
- for the employee’s reporting structure
- and request is currently pending at their approval level

---

## Validation Rules

### Leave Apply
- Start date must be valid
- End date must be valid
- No overlapping leave
- Balance should be sufficient unless leave type allows negative balance
- Half-day should follow leave type policy
- Holidays/weekends counted based on policy
- Approvers must be resolvable from hierarchy

### Leave Approval
- Only valid approver can approve/reject
- Only correct approval level can act
- Already finalized request cannot be re-approved
- Comments mandatory on rejection if policy requires

### Leave Cancellation
- Only owner or authorized role can cancel
- Already cancelled requests cannot be cancelled again
- Balance reversal required for approved leave

---

## API Expectations

### Auth
- `POST /api/auth/login`
- `POST /api/auth/refresh-token`

### Users / Employees
- `GET /api/users`
- `POST /api/users`
- `PUT /api/users/{id}`
- `GET /api/employees/{id}`

### Leave
- `POST /api/leaves`
- `GET /api/leaves/my`
- `GET /api/leaves/team-pending`
- `GET /api/leaves/{id}`
- `POST /api/leaves/{id}/approve`
- `POST /api/leaves/{id}/reject`
- `POST /api/leaves/{id}/cancel`

### Dashboard
- `GET /api/dashboard/employee`
- `GET /api/dashboard/manager`
- `GET /api/dashboard/hr`
- `GET /api/dashboard/admin`

### Reports
- `GET /api/reports/leave-balance`
- `GET /api/reports/department-summary`
- `GET /api/reports/monthly-utilization`
- `GET /api/reports/approval-summary`

---

## Coding Standards

### General
- Use clear, descriptive naming
- Keep files small and single-purpose
- Prefer composition over duplication
- Avoid hardcoding business rules in controllers
- Keep business rules in domain/application layer
- Write readable and testable code

### Backend
- Controllers should be thin
- Services/handlers should contain business logic
- Validation should be centralized
- Repository or DbContext usage should be consistent
- Use async/await properly
- Use DTOs for request/response models
- Do not expose database entities directly in API responses

### Frontend
- Build reusable UI components
- Keep API logic in service layer
- Keep feature logic inside feature folders
- Protect routes by role/access
- Use form validation consistently
- Keep dashboard widgets modular

---

## Testing Expectations

### Backend Tests
- Unit tests for services / handlers
- Validation tests
- Approval workflow tests
- Balance calculation tests
- Authorization tests

### Frontend Tests
- Component tests for important forms and dashboards
- Workflow tests for leave apply and approval pages

### Critical Test Scenarios
1. Employee applies leave with valid balance
2. Employee applies overlapping leave
3. Level 1 approves and moves to Level 2
4. Level 2 approves and finalizes leave
5. Approver rejects request
6. Unauthorized user tries to approve
7. Approved leave cancellation restores balance
8. Dashboard only shows role-appropriate data

---

## Agent Definitions

This project should be executed with the following conceptual agents.

### 1. Solution Architect Agent
Responsibilities:
- Define architecture
- Decide module boundaries
- Ensure scalability and maintainability
- Review domain model and approval design

### 2. Backend Agent
Responsibilities:
- Build ASP.NET Core Web API
- Implement auth, roles, hierarchy, leave workflow
- Implement reports APIs
- Build audit and notification services

### 3. Frontend Agent
Responsibilities:
- Build React UI
- Implement dashboards and forms
- Implement route protection and state management
- Integrate charts and MIS screens

### 4. Database Agent
Responsibilities:
- Design MySQL schema
- Create indexes and constraints
- Support efficient reporting queries
- Maintain migration consistency

### 5. QA Agent
Responsibilities:
- Validate workflows
- Verify role/hierarchy-based access
- Test approval levels and balance changes
- Confirm dashboard/report correctness

### 6. DevOps Agent
Responsibilities:
- Setup environments
- Manage CI/CD
- Configure logging, secrets, deployment, backups

---

## Agent Operating Rules

All agents must follow these rules:

1. Preserve business rules around approval hierarchy.
2. Do not assume every approver is Admin or HR.
3. Respect distinction between:
   - system role
   - approval responsibility
4. Keep workflow configurable up to 2 levels.
5. Every approval/rejection must be auditable.
6. Dashboard data must be role-specific.
7. MIS queries must be optimized for real-world use.
8. Avoid breaking API contracts once established.
9. Prefer incremental delivery with working modules.
10. Keep security and authorization as first-class requirements.

---

## Delivery Phases

### Phase 1
- Authentication
- User/employee setup
- Reporting hierarchy
- Leave type master
- Apply leave
- 1-level and 2-level approval workflow
- Basic dashboards

### Phase 2
- Leave balance engine
- Holiday calendar
- MIS reports
- Notifications
- Approval history
- Audit logs

### Phase 3
- Advanced leave policies
- Delegation/escalation
- Enhanced dashboards
- Export improvements
- Performance tuning
- Mobile responsiveness

---

## Out of Scope for MVP
Unless explicitly requested, avoid including these in first release:
- Payroll integration
- Biometric attendance integration
- Multi-tenant architecture
- Mobile app
- Complex workflow engine beyond 2 levels
- AI forecasting features

---

## Success Criteria

The project is successful when:
- Employees can apply leave smoothly
- Managers can approve subordinate leave based on hierarchy
- Workflow supports up to 2 levels correctly
- HR/Admin can operate and report effectively
- Dashboards provide meaningful user-specific insights
- MIS exports are useful and accurate
- Security and audit requirements are met

---

## Final Instruction to All Agents

When in doubt:
- prioritize correctness of approval hierarchy
- keep business logic explicit
- avoid overengineering
- deliver a maintainable MVP first
- ensure the system is easy to extend later
