# Template Starter for DB Applications

Reusable starter structure to build any database-driven application with low setup effort.

## Includes
- Layered backend skeleton (`Api`, `Application`, `Domain`, `Infrastructure`)
- Frontend modular skeleton (`app`, `components`, `features`, `services`)
- Mandatory docs templates (`docs/`)
- Dev scripts (`bootstrap`, `migrate`, `seed`, `new-module`)
- `.env.example` and `docker-compose.yml` templates

## Quick Start
1. Copy this folder as your new project base.
2. Rename solution/module names as needed.
3. Fill `.env` from `.env.example`.
4. Implement schema in `docs/schema.sql`.
5. Add entities + DTOs + APIs feature by feature.

## Module Generator
Use the scaffold generator to create a new module in one step.

### Command
```powershell
pwsh ./scripts/new-module.ps1 -ModuleName "LeaveType" -IncludeController -IncludeFrontend
```

### Generated Files
- `backend/src/Domain/Entities/<Module>.cs`
- `backend/src/Application/<Module>/<Module>Dto.cs`
- `backend/src/Application/Abstractions/I<Module>Service.cs`
- `backend/src/Infrastructure/Services/<Module>Service.cs`
- `backend/src/Infrastructure/Persistence/Configurations/<Module>Configuration.cs`
- Optional: `backend/src/Api/Controllers/<Module>Controller.cs`
- Optional: `frontend/src/features/<module-kebab>/pages/<Module>Page.jsx`

### Post-Generation Checklist
1. Register `DbSet` in `AppDbContext`.
2. Register service in DI (`ServiceCollectionExtensions`).
3. Add API route in frontend routes.
4. Add EF migration.
5. Update `docs/api-contract.md`, `docs/domain-model.md`, and `docs/test-plan.md`.

## Suggested Development Flow
1. Complete docs first (`requirements`, `schema`, `api-contract`, `workflow`).
2. Build DB schema and migrations.
3. Add auth and user module.
4. Add feature modules one by one.
5. Add tests for each module.

## Folder Map
- `backend/` - API and business logic layers.
- `frontend/` - UI app with feature-based modules.
- `docs/` - planning, contracts, test and decision logs.
- `scripts/` - local setup helpers.

## Notes
- Keep schema as source of truth.
- Keep traceability: Requirement -> API -> DB -> UI -> Test.
- Keep secrets out of git.
