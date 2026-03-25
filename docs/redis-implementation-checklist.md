# Redis Implementation Checklist (Code-Level)

This checklist is for the **current .NET solution** under `backend/src`.

## 0. Scope and Principles
- Use Redis for **read-heavy, non-sensitive** data first.
- Keep DB as source of truth.
- Add deterministic cache invalidation on writes.
- Add metrics (hit/miss/invalidate) from day 1.

---

## 1. NuGet + Configuration

### 1.1 Add package
- [ ] Add `Microsoft.Extensions.Caching.StackExchangeRedis` to:
  - `backend/src/LeaveManagement.Infrastructure/LeaveManagement.Infrastructure.csproj`

### 1.2 Add options class
- [ ] Create file: `backend/src/LeaveManagement.Infrastructure/Caching/RedisCacheOptions.cs`
- [ ] Add fields:
  - `ConnectionString`
  - `InstanceName`
  - `DefaultTtlSeconds`
  - `Enabled`

### 1.3 Add appsettings
- [ ] Update `backend/src/LeaveManagement.API/appsettings.json`
- [ ] Add section:
```json
"Redis": {
  "Enabled": true,
  "ConnectionString": "localhost:6379",
  "InstanceName": "lms:",
  "DefaultTtlSeconds": 300
}
```
- [ ] Add same keys in `appsettings.Development.json` (if present).

### 1.4 Register distributed cache
- [ ] Update `backend/src/LeaveManagement.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`
- [ ] Bind `Redis` section to `RedisCacheOptions`.
- [ ] If enabled: `services.AddStackExchangeRedisCache(...)`
- [ ] If disabled: keep memory fallback via `AddDistributedMemoryCache()`.

---

## 2. Cache Abstraction Layer

### 2.1 Create cache service contract
- [ ] Add `backend/src/LeaveManagement.Application/Abstractions/IAppCache.cs`
- [ ] Methods:
  - `Task<T?> GetAsync<T>(string key, CancellationToken ct)`
  - `Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken ct)`
  - `Task RemoveAsync(string key, CancellationToken ct)`
  - `Task RemoveByPrefixAsync(string prefix, CancellationToken ct)` (best effort)

### 2.2 Implement cache service
- [ ] Add `backend/src/LeaveManagement.Infrastructure/Caching/RedisAppCache.cs`
- [ ] Use `IDistributedCache` + `System.Text.Json`.
- [ ] Add structured log events:
  - cache_hit
  - cache_miss
  - cache_set
  - cache_remove

### 2.3 Register implementation
- [ ] In `ServiceCollectionExtensions.cs` add:
  - `services.AddScoped<IAppCache, RedisAppCache>();`

---

## 3. Cache Key Strategy

### 3.1 Add centralized key builder
- [ ] Create `backend/src/LeaveManagement.Infrastructure/Caching/CacheKeys.cs`
- [ ] Add static methods:
  - `MastersDepartments()`
  - `MastersDesignations()`
  - `MastersDepartmentDesignationMap()`
  - `MastersLeaveTypes()`
  - `MastersHolidays()`
  - `UserProfile(Guid userId)`
  - `Hierarchy(Guid employeeId)`
  - `DashboardByRole(string role, Guid? employeeId)`
  - `LeaveBalances(Guid employeeId)`

### 3.2 TTL defaults
- [ ] Add `CacheTtls.cs` with constants:
  - masters: `30m`
  - dashboard: `2m`
  - hierarchy: `10m`
  - leave balance: `2m`
  - user profile: `10m`

---

## 4. First Caching Targets (Low Risk)

## 4.1 Master data service
- [ ] Update `backend/src/LeaveManagement.Infrastructure/Services/MasterDataService.cs`
- [ ] Cache reads:
  - departments
  - designations
  - maps
  - leave types
  - holidays
- [ ] Invalidate related keys on create/update/delete methods.

## 4.2 User profile
- [ ] Update `backend/src/LeaveManagement.Infrastructure/Services/UserManagementService.cs`
- [ ] Cache `GetMyProfile`/profile read methods.
- [ ] Invalidate on profile update.

## 4.3 Hierarchy
- [ ] Update `backend/src/LeaveManagement.Infrastructure/Services/HierarchyService.cs` (if this file exists in current codebase)
- [ ] Cache hierarchy read by employee.
- [ ] Invalidate on upsert/update hierarchy.

## 4.4 Dashboard
- [ ] Update `backend/src/LeaveManagement.Infrastructure/Services/DashboardService.cs` (if present)
- [ ] Cache role-wise dashboard summaries.
- [ ] Invalidate on leave state changes.

---

## 5. Write-Path Invalidation (Critical)

### 5.1 Leave workflow events
- [ ] Update `backend/src/LeaveManagement.Infrastructure/Services/LeaveRequestService.cs`
- [ ] Update `backend/src/LeaveManagement.Infrastructure/Services/LeaveBalanceService.cs`
- [ ] After apply/approve/reject/cancel, invalidate:
  - requester leave balances
  - requester dashboard
  - approver dashboard(s)
  - pending approvals snapshots (if cached)

### 5.2 Master writes
- [ ] In all master write methods, remove impacted master keys immediately after successful DB commit.

### 5.3 User/role/hierarchy writes
- [ ] On role changes and hierarchy updates, invalidate profile/menu/hierarchy keys.

---

## 6. Controller-Level Response Cache (Optional, Fast Win)
- [ ] For safe GET-only endpoints, add response caching headers in controllers:
  - `MastersController`
  - dashboard GET endpoints
- [ ] Do NOT cache auth/login or user-sensitive responses in shared caches.

---

## 7. Safety and Correctness

### 7.1 Serialization guard
- [ ] Use versioned payload contract in cached values:
  - e.g., wrapper `{ version, payload }`

### 7.2 Null caching policy
- [ ] Decide and implement:
  - cache null for short TTL (to prevent repeated DB misses)
  - or skip null caching

### 7.3 Stampede mitigation
- [ ] Add jitter to TTL (`ttl +/- random seconds`).
- [ ] Optional: per-key lock for expensive rebuilds.

---

## 8. Diagnostics and Observability

### 8.1 Metrics
- [ ] Expose counters (or logs initially):
  - hits
  - misses
  - sets
  - invalidations

### 8.2 Logging
- [ ] Add request correlation id to cache logs.
- [ ] Add warning log when Redis unavailable and fallback used.

### 8.3 Health check
- [ ] Add Redis health check in API startup:
  - file: `backend/src/LeaveManagement.API/Program.cs`

---

## 9. Tests (Must Have)

### 9.1 Unit tests
- [ ] Add tests in `backend/tests/LeaveManagement.Tests` for:
  - key generation correctness
  - ttl policy selection
  - cache wrapper serialization

### 9.2 Integration tests
- [ ] Add tests for:
  - cached read returns same payload as DB path
  - write operation invalidates affected key
  - stale data not returned post-update

### 9.3 Non-functional test
- [ ] Compare p95 latency before/after for:
  - masters GET
  - dashboard GET
  - hierarchy GET

---

## 10. Rollout Plan (Safe)
1. [ ] Deploy with Redis disabled (`Enabled=false`) and verify no regression.
2. [ ] Enable caching only for master reads.
3. [ ] Enable user profile + hierarchy cache.
4. [ ] Enable dashboard + leave balance cache.
5. [ ] Monitor hit ratio and stale-data incidents.
6. [ ] Tune TTL and invalidation based on observed patterns.

---

## 11. Suggested Initial PR Breakdown

### PR-1: Infrastructure only
- Redis config/options
- `IAppCache` + implementation
- key and ttl helpers

### PR-2: Master reads/writes caching
- `MasterDataService` cache + invalidation
- tests

### PR-3: User + hierarchy caching
- `UserManagementService` + hierarchy service
- tests

### PR-4: Dashboard + leave invalidation
- dashboard cache
- leave apply/approve/reject/cancel invalidation hooks
- tests + latency benchmark notes

---

## 12. Done Criteria
- [ ] No functional behavior changes with cache OFF.
- [ ] Cache ON improves p95 latency for selected GET endpoints.
- [ ] No stale data bug in critical workflows.
- [ ] Hit ratio measurable and documented.
- [ ] Rollback path documented (`Enabled=false`).
