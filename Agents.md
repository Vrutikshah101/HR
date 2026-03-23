# AGENTS.md

## Project Name
Leave Management System

---

## Project Overview

This system is a **Leave Management System** built using:

- Backend: ASP.NET Core Web API
- Frontend: React.js
- Database: MySQL

Primary roles:
- Admin
- HR
- Employee/User

Special rule:
A normal user can act as an **approver** if configured in reporting hierarchy.

Approval flow:
- 1-level or 2-level approval

---

## Core Principle

This project is designed to be:

- **Low hallucination**
- **Highly testable**
- **Phase-driven**
- **Traceable**
- **Production-ready**

---

# 🚫 Anti-Hallucination Policy

All agents MUST follow:

### 1. No invention allowed
Do NOT invent:
- tables
- columns
- APIs
- workflows
- statuses
- dashboard metrics

If missing → log in `assumptions.md`

---

### 2. Separate truth vs assumption
Every unclear item must be labeled:
- Confirmed
- Assumption
- Open Question

---

### 3. Schema is source of truth
- DB schema controls API + logic
- No schema → no implementation

---

### 4. No hidden logic
Business logic must NOT live in:
- controllers
- UI
- random helpers

---

### 5. Every feature must map to:
- requirement
- schema
- API
- test

---

### 6. No fake completion
“Done” only if:
- implemented
- tested
- validated

---

# 📁 Required Documentation (MANDATORY)

All files must exist under `/docs`:

- requirements.md
- domain-model.md
- schema.sql
- api-contract.md
- workflow.md
- access-matrix.md
- assumptions.md
- decision-log.md
- test-plan.md
- phase-plan.md

---

# 📊 Traceability Rule

Every feature must map:

| Requirement | API | DB | UI | Test |
|------------|-----|----|----|------|

No mapping → not allowed to implement

---

# 🚀 Phase Execution Model

---

## Phase -1: Planning & Documentation (MANDATORY)

### Objective
Eliminate ambiguity.

### Scope
- requirements
- schema
- API contracts
- workflows
- roles
- dashboards
- assumptions
- decisions
- test plan

### Exit Criteria
- no undefined entities
- no unclear workflow
- schema exists
- APIs drafted
- assumptions logged

🚫 No coding allowed before this phase completes

---

## Phase 0: Docker & Environment Setup

### Scope
- Docker
- docker-compose
- backend container
- frontend container
- MySQL container
- env config

### Exit Criteria
- single command runs system
- API reachable
- frontend loads
- DB connected

---

## Phase 1: UI/UX Only (No Backend Logic)

### Scope
- layouts
- dashboards (static)
- forms
- navigation
- reusable components

### Exit Criteria
- all screens visible
- responsive UI
- role-based menus (mocked)

---

## Phase 2: DB Schema + Backend Foundation

### Scope
- schema implementation
- EF Core setup
- migrations
- architecture layers

### Exit Criteria
- schema applied
- migrations working
- API base ready

---

## Phase 3: Auth + Users + Hierarchy

### Scope
- login
- JWT
- users
- roles
- employees
- hierarchy

### Exit Criteria
- login works
- role-based access works
- hierarchy stored correctly

---

## Phase 4: Leave Apply

### Scope
- leave types
- apply leave
- validation
- history

### Exit Criteria
- leave applied successfully
- validation works
- data stored correctly

---

## Phase 5: Approval Workflow (Core Phase)

### Scope
- level 1 approval
- level 2 approval
- rejection
- audit trail

### Exit Criteria
- correct approver only
- correct level only
- audit logged
- workflow tested fully

---

## Phase 6: Leave Balance + Holidays

### Scope
- balance tracking
- deduction
- cancellation
- holiday logic

### Exit Criteria
- balance accurate
- cancellation restores balance
- holidays handled

---

## Phase 7: Dashboards

### Scope
- role-based dashboards
- KPI APIs
- charts

### Exit Criteria
- each metric defined
- values verified against DB
- role visibility correct

---

## Phase 8: MIS Reports

### Scope
- reports
- filters
- export

### Exit Criteria
- data correct
- filters correct
- export matches UI

---

## Phase 9: Notifications + Hardening

### Scope
- notifications
- audit improvement
- security
- logging

### Exit Criteria
- all key actions logged
- notifications triggered

---

## Phase 10: QA & Release

### Scope
- E2E testing
- regression
- bug fixes
- documentation

### Exit Criteria
- no critical bugs
- flows verified
- release ready

---

# 🧪 Testing Policy

Every phase MUST include:

- unit tests
- integration tests
- API tests
- DB validation

Critical areas MUST be tested:
- approval workflow
- hierarchy validation
- balance logic
- authorization
- reports

---

# ✅ Definition of Done

A feature is done ONLY if:

1. requirement exists
2. schema supports it
3. API implemented
4. UI implemented
5. validation implemented
6. tests written
7. tests passed
8. audit considered
9. docs updated
10. no critical bugs

---

# 🔐 Access Control Rules

- Employee → own data only
- Approver → subordinate data
- HR/Admin → broader access

Approval allowed ONLY if:
- user is Level1 or Level2 approver
- correct level
- correct status

---

# 📊 Dashboard Rules

Every metric MUST define:
- source tables
- filters
- formula
- role visibility

No definition → metric invalid

---

# 🧱 Database Policy

1. Schema is authoritative
2. No table/column without:
   - migration
   - documentation
3. All APIs must map to schema
4. Changes require:
   - decision log
   - test update

---

# 👥 Agents

## Architect
- design system
- validate domain
- approve schema

## Backend
- APIs
- workflow
- validation

## Frontend
- UI
- dashboards
- forms

## Database
- schema
- performance
- integrity

## QA
- tests
- validation
- phase signoff

## DevOps
- CI/CD
- deployment
- environments

---

# 📋 Phase Signoff Format

Each phase must produce:

- scope completed
- tests written
- tests passed
- defects
- risks
- status

Status:
- Passed
- Passed with notes
- Blocked

---

# ⚠️ Final Rule

If unsure:
1. do NOT guess
2. log assumption
3. implement minimal version
4. test before marking done

---

# 🧭 Development Order (Final)

1. Phase -1 → Planning
2. Phase 0 → Docker
3. Phase 1 → UI
4. Phase 2 → Schema + backend base
5. Phase 3 → Auth + users
6. Phase 4 → Leave apply
7. Phase 5 → Approval workflow
8. Phase 6 → Balance
9. Phase 7 → Dashboard
10. Phase 8 → MIS
11. Phase 9 → Hardening
12. Phase 10 → QA

---

## Success Criteria

- No hallucinated logic
- Approval workflow correct
- Reports accurate
- Dashboards reliable
- System fully testable
- Clean, extendable architecture
