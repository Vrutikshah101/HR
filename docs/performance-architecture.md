# Performance Architecture Plan

## 1. Objective
Build a high-performance, production-grade Leave Management platform with:
- low-latency APIs
- predictable throughput under load
- scalable architecture evolution
- strong observability and resilience

## 2. Guiding Approach
1. Optimize current architecture first (modular monolith + cache + DB tuning).
2. Split into microservices only where metrics justify separation.
3. Add observability before deep optimization so improvements are measurable.
4. Roll out in phases with rollback-safe changes.

## 3. Current Baseline (Assumed)
- ASP.NET Core API
- React frontend
- MySQL
- JWT-based auth
- Core leave workflows implemented

## 4. Target Architecture (Phased)

### Stage A: High-Performance Monolith
- Redis distributed cache
- API response caching
- optimized DB indexes and query plans
- background jobs for non-blocking tasks

### Stage B: Event-Driven Extensions
- message broker for async workflows
- outbox pattern for reliable event publishing
- activity/notification/report generation off request path

### Stage C: Selective Microservices
- reporting service extraction
- notification service extraction
- optional master-data service extraction

### Stage D: Platform Hardening
- API gateway
- autoscaling strategy
- read replicas for MySQL
- SLO monitoring and error-budget governance

## 5. Caching Strategy (All Levels)

### 5.1 Client/Browser Layer
- use cache-control headers for static assets
- use ETag for GET endpoints where feasible
- avoid caching sensitive user-specific payloads in shared proxies

### 5.2 Edge/CDN Layer
- cache immutable frontend bundles aggressively
- cache public metadata endpoints if introduced

### 5.3 API Response Caching
Cache candidates:
- master lists (departments, designations, leave types, holidays)
- dashboard summary endpoints (short TTL)
- non-sensitive report metadata

### 5.4 Application Data Caching (Redis)
- user profile summary
- role/menu payload
- hierarchy lookup
- leave balance snapshot

### 5.5 Query Optimization Layer
- EF compiled queries for repeated hot paths
- proper covering indexes for filter/sort columns
- avoid N+1 and over-fetching via projection

## 6. Redis Design

### 6.1 Key Convention
`lms:{env}:{module}:{entity}:{id}`

Examples:
- `lms:dev:master:departments:all`
- `lms:dev:user:profile:{userId}`
- `lms:dev:dashboard:summary:{employeeId}`

### 6.2 TTL Matrix
- master data: 30-60 min
- dashboard aggregates: 1-5 min
- user profile/menu: 10-30 min
- leave balance snapshot: 1-3 min

### 6.3 Invalidation Rules
- on create/update/delete of master records -> invalidate master keys
- on leave apply/approve/reject/cancel -> invalidate leave balance + dashboard keys
- on role/hierarchy change -> invalidate user profile/menu/hierarchy keys

### 6.4 Stampede Protection
- soft TTL + background refresh
- distributed lock for rebuild of heavy keys
- random jitter on TTL to avoid synchronized expiry

## 7. Microservices Decomposition Plan

### Service Candidates
1. Identity/Auth Service
2. Leave Workflow Service
3. Master Data Service
4. Reporting Service
5. Notification Service

### Recommended Extraction Order
1. Notification Service (lowest coupling)
2. Reporting Service (read-heavy, async-friendly)
3. Master Data Service (if scale/team boundary requires)
4. Leave Workflow and Identity only when traffic/organizational scale demands

## 8. Messaging and Async Processing

### Broker Options
- RabbitMQ (simpler operations)
- Kafka (high-throughput stream processing)

### Event Examples
- `LeaveApplied`
- `LeaveApproved`
- `LeaveRejected`
- `UserRegistered`
- `HierarchyUpdated`

### Patterns
- Outbox table in core DB
- idempotent consumers
- dead-letter queue + retry policy

## 9. Database Performance Plan

### 9.1 Indexing
Ensure indexes on:
- leave requests (`employee_id`, `status`, `created_at_utc`)
- approvals (`request_id`, `level`, `actioned_at_utc`)
- users (`email`, `employee_code`, `department`, `designation`)
- hierarchy (`employee_id`, `level1_approver_id`, `level2_approver_id`)

### 9.2 Query Efficiency
- no full table scans on dashboard/report endpoints
- use pagination for large grids
- use projections instead of loading full entities

### 9.3 Scale Strategy
- connection pooling tuning
- read replicas for reporting
- partition archival strategy for activity/audit logs

## 10. Resilience and Security
- Polly policies: retry + circuit breaker + timeout + fallback
- rate limiting by endpoint class
- secure cache content (no sensitive token payloads in shared cache)
- centralized exception handling with correlation IDs

## 11. Observability and SRE

### 11.1 Metrics
- p50/p95/p99 latency per endpoint
- requests per second
- cache hit ratio per key group
- DB query time and slow query count
- queue depth and consumer lag

### 11.2 Tracing
- OpenTelemetry traces across API, DB, Redis, message broker
- distributed trace propagation with correlation IDs

### 11.3 Logging
- structured logs (JSON)
- log levels by environment
- audit and operational logs separated

## 12. Load and Capacity Testing
- tool: k6 or JMeter
- baseline scenarios:
  - login burst
  - leave apply burst
  - approval burst
  - dashboard/report concurrent reads
- report throughput, latency, and error rate before and after each phase

## 13. Implementation Roadmap

### Phase A (2-3 weeks)
- integrate Redis
- cache master + dashboard endpoints
- add invalidation hooks
- add key DB indexes
- define baseline load tests

### Phase B (2-4 weeks)
- add OpenTelemetry + Prometheus + Grafana
- add background jobs and async notification pipeline
- introduce outbox pattern

### Phase C (3-6 weeks)
- extract reporting service
- extract notification service
- route via API gateway

### Phase D (ongoing)
- autoscaling policies
- read replica strategy
- SLO/error-budget operations

## 14. Definition of Performance Done
A phase is complete only if:
1. target latency and throughput are defined
2. changes are implemented and deployed
3. load test shows measurable improvement
4. cache hit ratio targets are met
5. rollback and incident runbook are updated

## 15. Open Decisions
1. Redis hosting mode (self-managed vs managed cloud)
2. Broker selection (RabbitMQ vs Kafka)
3. API gateway choice (YARP/Kong/WSO2)
4. tracing backend (Jaeger vs Tempo)
5. service extraction trigger thresholds (RPS, p99, team ownership)
