# Leave Management System

MVP scaffold for a role-based leave management product with hierarchy-driven approvals (up to 2 levels).

## Repository Structure

- `backend/src/LeaveManagement.API`: ASP.NET Core Web API entrypoint and controllers
- `backend/src/LeaveManagement.Application`: application services/contracts
- `backend/src/LeaveManagement.Domain`: domain entities and enums
- `backend/src/LeaveManagement.Infrastructure`: persistence/auth implementations
- `frontend`: React feature-based UI scaffold

## Current Status

- Clean architecture folder layout created
- Core domain models and workflow enums added
- API contract DTOs and placeholder endpoints added
- Frontend feature/module structure with routed placeholder pages added

## Prerequisites

- .NET SDK 8.0+ (required to build backend)
- Node.js 20+ and npm (required to run frontend)

## Next Steps

1. Install .NET SDK and wire EF Core + MySQL migrations.
2. Implement JWT auth and role/hierarchy authorization policies.
3. Implement leave apply/approve/reject/cancel workflow service with tests.
4. Replace frontend placeholder pages with integrated API-driven screens.
