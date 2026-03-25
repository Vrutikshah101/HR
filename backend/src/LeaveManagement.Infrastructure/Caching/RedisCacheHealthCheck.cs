using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LeaveManagement.Infrastructure.Caching;

public sealed class RedisCacheHealthCheck : IHealthCheck
{
    private readonly IDistributedCache _cache;

    public RedisCacheHealthCheck(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"health:cache:{Guid.NewGuid():N}";
            var expected = "ok";

            await _cache.SetStringAsync(key, expected, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
            }, cancellationToken);

            var actual = await _cache.GetStringAsync(key, cancellationToken);
            await _cache.RemoveAsync(key, cancellationToken);

            return actual == expected
                ? HealthCheckResult.Healthy("Cache read/write check passed.")
                : HealthCheckResult.Unhealthy("Cache read/write check failed.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cache health check failed.", ex);
        }
    }
}
