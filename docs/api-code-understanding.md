# API Code Understanding Guide

This guide maps each API endpoint to controller methods and service methods in the current codebase.

## 1. Request Flow (How to Read Code)
1. Route enters controller (`backend/src/LeaveManagement.API/Controllers/*Controller.cs`).
2. Controller maps request DTO to application command/query.
3. Controller calls service interface from `LeaveManagement.Application/Abstractions`.
4. Service implementation in `LeaveManagement.Infrastructure/Services` performs validation + DB actions.
5. Response DTO is returned from controller.

## 2. Controllers and Responsibilities
- `AuthController`: login/token issuance.
- `UsersController`: user registration, list users, self profile read/update.
- `HierarchyController`: reporting hierarchy upsert/get.
- `MastersController`: departments, designations, mappings, leave types, holidays.
- `LeavesController`: leave apply/list/details, approval actions, cancellation, leave balances, holidays.
- `DashboardController`: role-based KPI cards.
- `ReportsController`: MIS reporting and CSV export.

## 3. Endpoint Mapping

| Endpoint | Controller Method | Service Method(s) | Purpose |
|---|---|---|---|
| `POST /api/auth/login` | `AuthController.Login` | `IAuthService.LoginAsync` | Validate credentials and return JWT tokens |
| `POST /api/users` | `UsersController.Create` | `IUserManagementService.CreateUserAsync` | Create user + employee profile (Admin/Hr) |
| `GET /api/users` | `UsersController.GetAll` | `IUserManagementService.GetUsersAsync` | List users (Admin/Hr) |
| `GET /api/users/me` | `UsersController.GetMe` | `IUserManagementService.GetMyProfileAsync` | Get current user profile |
| `PUT /api/users/me` | `UsersController.UpdateMe` | `IUserManagementService.UpdateMyProfileAsync` | Update current profile |
| `PUT /api/hierarchy` | `HierarchyController.Upsert` | `IHierarchyService.UpsertAsync` | Set/Update L1/L2 approvers |
| `GET /api/hierarchy/{employeeId}` | `HierarchyController.GetByEmployeeId` | `IHierarchyService.GetByEmployeeIdAsync` | Get approver chain for employee |
| `GET /api/masters/departments` | `MastersController.GetDepartments` | `IMasterDataService.GetDepartmentsAsync` | Department master list |
| `POST /api/masters/departments` | `MastersController.CreateDepartment` | `IMasterDataService.CreateDepartmentAsync` | Create department |
| `GET /api/masters/designations` | `MastersController.GetDesignations` | `IMasterDataService.GetDesignationsAsync` | Designation master list |
| `GET /api/masters/departments/{id}/designations` | `MastersController.GetDesignationsByDepartment` | `IMasterDataService.GetDesignationsByDepartmentAsync` | Cascading designations by department |
| `POST /api/masters/designations` | `MastersController.CreateDesignation` | `IMasterDataService.CreateDesignationAsync` | Create designation |
| `GET /api/masters/department-designation-maps` | `MastersController.GetDepartmentDesignationMaps` | `IMasterDataService.GetDepartmentDesignationMapsAsync` | Department-designation relation list |
| `POST /api/masters/department-designation-maps` | `MastersController.CreateDepartmentDesignationMap` | `IMasterDataService.CreateDepartmentDesignationMapAsync` | Create relation mapping |
| `GET /api/masters/leave-types` | `MastersController.GetLeaveTypes` | `IMasterDataService.GetLeaveTypesAsync` | Leave type master list |
| `POST /api/masters/leave-types` | `MastersController.CreateLeaveType` | `IMasterDataService.CreateLeaveTypeAsync` | Create leave type |
| `GET /api/masters/holidays` | `MastersController.GetHolidays` | `IMasterDataService.GetHolidaysAsync` | Holiday master list |
| `POST /api/masters/holidays` | `MastersController.CreateHoliday` | `IMasterDataService.CreateHolidayAsync` | Create holiday |
| `GET /api/leaves/types` | `LeavesController.GetTypes` | `ILeaveRequestService.GetLeaveTypesAsync` | Leave types for apply form |
| `GET /api/leaves/holidays` | `LeavesController.GetHolidays` | `ILeaveBalanceService.GetHolidaysAsync` | Holiday dates for UI |
| `GET /api/leaves/my-balances` | `LeavesController.GetMyBalances` | `ILeaveBalanceService.GetMyBalancesAsync` | Current user leave balances |
| `POST /api/leaves` | `LeavesController.Apply` | `ILeaveRequestService.ApplyAsync` | Apply leave with validations |
| `GET /api/leaves/my` | `LeavesController.GetMy` | `ILeaveRequestService.GetMyAsync` | My leave history |
| `GET /api/leaves/team-pending` | `LeavesController.GetTeamPending` | `ILeaveRequestService.GetTeamPendingAsync` | Pending leaves for approver |
| `GET /api/leaves/{id}` | `LeavesController.GetById` | `ILeaveRequestService.GetByIdAsync` | Leave details with access checks |
| `POST /api/leaves/{id}/approve` | `LeavesController.Approve` | `ILeaveWorkflowService.ApproveAsync` | Approve at current workflow level |
| `POST /api/leaves/{id}/reject` | `LeavesController.Reject` | `ILeaveWorkflowService.RejectAsync` | Reject leave (comment required) |
| `POST /api/leaves/{id}/cancel` | `LeavesController.Cancel` | `ILeaveRequestService.CancelAsync` | Cancel pending/approved leave |
| `GET /api/dashboard/employee` | `DashboardController.Employee` | `IDashboardService.GetEmployeeCardsAsync` | Employee dashboard KPIs |
| `GET /api/dashboard/manager` | `DashboardController.Manager` | `IDashboardService.GetManagerCardsAsync` | Manager dashboard KPIs |
| `GET /api/dashboard/hr` | `DashboardController.Hr` | `IDashboardService.GetHrCardsAsync` | HR dashboard KPIs |
| `GET /api/dashboard/admin` | `DashboardController.Admin` | `IDashboardService.GetAdminCardsAsync` | Admin dashboard KPIs |
| `GET /api/reports/leave-balance` | `ReportsController.LeaveBalance` | `IReportService.GetLeaveBalanceAsync` | Leave balance report |
| `GET /api/reports/department-summary` | `ReportsController.DepartmentSummary` | `IReportService.GetDepartmentSummaryAsync` | Dept summary report |
| `GET /api/reports/monthly-utilization` | `ReportsController.MonthlyUtilization` | `IReportService.GetMonthlyUtilizationAsync` | Monthly utilization report |
| `GET /api/reports/approval-summary` | `ReportsController.ApprovalSummary` | `IReportService.GetApprovalSummaryAsync` | Approval activity summary |

## 4. Key Business Rule Locations
- Leave apply validations: `LeaveRequestService.ApplyAsync`
- Approval authorization + transitions: `LeaveWorkflowService.HandleActionAsync`
- Leave balance deduction/restore: `LeaveBalanceService.ApplyApprovedDeductionAsync`, `LeaveBalanceService.RestoreForCancellationAsync`
- Hierarchy validation: `HierarchyService.UpsertAsync`
- User password and role validation: `UserManagementService.CreateUserAsync`

## 5. Caching-Related Code (Current)
- Cache abstraction: `IAppCache`
- Cache implementation: `RedisAppCache`
- Key map: `CacheKeys`
- TTL map: `CacheTtls`
- Health check endpoint: `GET /health`

## 6. How to Debug Any API Quickly
1. Start from controller action.
2. Inspect called service method in `Infrastructure/Services`.
3. Check EF queries and entity updates.
4. Check invalidation calls to `IAppCache`.
5. Verify response mapping in controller.
