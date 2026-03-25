# WBS and Project Plan

## Project
Performance Optimization and Scalability Upgrade for Leave Management System

## Planning Assumptions
- Team: 1 Backend Engineer + 1 Fullstack Engineer + 1 QA (shared) + 1 DevOps (part-time)
- Current architecture remains operational during implementation.
- Delivery strategy: incremental, backward compatible, feature flags/config toggles.
- Timeline below is in **working days**.

---

## 1. Work Breakdown Structure (WBS)

### 1.0 Project Management and Governance
1.1 Kickoff, scope finalization, success criteria
1.2 Baseline metrics definition (latency, throughput, error rate)
1.3 Weekly review, risk tracking, stakeholder reporting
1.4 Go/No-Go decisions for each rollout gate

### 2.0 Foundation: Redis Enablement
2.1 Add Redis package and configuration model
2.2 Add cache abstraction (`IAppCache`) and implementation
2.3 Add DI registration with Redis/memory fallback
2.4 Add cache key naming and TTL standards
2.5 Add health check and startup validation

### 3.0 Application Caching Implementation
3.1 Cache master data reads (departments, designations, leave types, holidays)
3.2 Cache user profile and hierarchy reads
3.3 Cache dashboard summary endpoints
3.4 Cache leave balance snapshots
3.5 Add write-path invalidation hooks for all affected services

### 4.0 Reliability and Correctness
4.1 Null/value caching policy
4.2 TTL jitter and anti-stampede controls
4.3 Fallback behavior on Redis unavailability
4.4 Correlation-aware cache logging

### 5.0 Testing and Validation
5.1 Unit tests for key builder, ttl strategy, serializer
5.2 Integration tests for cache hit/miss and invalidation correctness
5.3 Regression testing of leave apply/approve/reject/cancel flows
5.4 Benchmark testing (baseline vs post-cache)

### 6.0 Observability and Operations
6.1 Cache metrics instrumentation (hit/miss/set/invalidate)
6.2 API and DB latency dashboards
6.3 Alert thresholds (error rate, p95 latency, Redis health)
6.4 Runbook and rollback (`Redis.Enabled=false`)

### 7.0 Phase-2 Architecture Preparation (Design Only)
7.1 Microservice decomposition candidate analysis
7.2 Event-driven architecture draft (outbox + broker)
7.3 Target deployment topology and gateway strategy
7.4 Capacity and cost estimation draft

---

## 2. Project Plan (Execution)

## Phase A: Redis Foundation and Safe Rollout
Duration: 3-4 days

Tasks:
- Implement WBS 2.1 to 2.5
- Validate API startup with Redis on/off
- Add initial operational runbook

Deliverables:
- Redis integrated with fallback
- Configurable and environment-safe cache subsystem

Exit Criteria:
- Application functions with Redis enabled and disabled
- No critical regression in existing APIs

## Phase B: Feature Caching and Invalidation
Duration: 4-6 days

Tasks:
- Implement WBS 3.1 to 3.5
- Add invalidation on write paths
- Add defensive logging around cache decisions

Deliverables:
- Master/User/Hierarchy/Dashboard/Balance caching in place
- Deterministic invalidation behavior

Exit Criteria:
- No stale data in critical flows
- Functional tests pass for cached and uncached paths

## Phase C: Test, Benchmark, and Stabilize
Duration: 3-4 days

Tasks:
- Implement WBS 4.1 to 6.4
- Run benchmark and compare baseline
- Tune TTLs and invalidation for real behavior

Deliverables:
- Test reports
- Performance delta report
- Monitoring and alert setup notes

Exit Criteria:
- p95 improvements demonstrated on selected endpoints
- Rollback path verified

## Phase D: Architecture Evolution Planning (Design Stage)
Duration: 2-3 days

Tasks:
- Implement WBS 7.1 to 7.4
- Prepare architecture decision proposal for next quarter

Deliverables:
- Microservice extraction candidates
- Event-driven reference design
- Capacity and cost draft

Exit Criteria:
- Stakeholder-approved architecture roadmap

---

## 3. Timeline Summary
- Phase A: 3-4 days
- Phase B: 4-6 days
- Phase C: 3-4 days
- Phase D: 2-3 days

Total: **12-17 working days** (single stream)

Parallelized team mode (Backend + QA/DevOps overlap): **8-12 working days**

---

## 4. Milestones
1. M1 - Redis Foundation Complete
2. M2 - Core Caching + Invalidation Complete
3. M3 - Performance Validation Complete
4. M4 - Next-Gen Architecture Proposal Complete

---

## 5. Dependency Map
- M2 depends on M1
- M3 depends on M2
- M4 can begin after M1, but final decisions should use M3 benchmark outputs

---

## 6. Resource Plan (RACI-lite)
- Backend Engineer: WBS 2, 3, 4 (Responsible)
- Fullstack Engineer: dashboard endpoint alignment, UI cache-safety checks (Responsible)
- QA Engineer: WBS 5 (Responsible)
- DevOps Engineer: WBS 6 and deployment runbook (Responsible)
- Tech Lead/Architect: WBS 1 and 7 (Accountable)

---

## 7. Risk Register (Initial)
1. Stale cache due to incomplete invalidation
   - Mitigation: write-path checklist + integration tests + short TTL in phase 1
2. Redis outage impacts API behavior
   - Mitigation: memory fallback + graceful degradation + health alerts
3. Performance gain below expectations
   - Mitigation: profile slow queries, add indexes, tune TTL and payload size
4. Scope creep into microservice implementation too early
   - Mitigation: keep Phase D design-only; no service split in this cycle

---

## 8. Success Metrics
- p95 latency reduction on target GET endpoints by >= 30%
- Cache hit ratio:
  - masters >= 85%
  - dashboard >= 70%
- No critical stale-data defects in UAT
- Error rate does not increase post rollout

---

## 9. Definition of Done
- All checklist items for selected scope completed
- Unit + integration + regression tests passed
- Benchmark report published
- Monitoring and rollback runbook documented
- Stakeholder signoff completed

---

## 10. Immediate Next Actions
1. Approve this WBS/plan baseline.
2. Start Phase A tasks from `docs/redis-implementation-checklist.md`.
3. Schedule benchmark baseline run before first cache rollout.
