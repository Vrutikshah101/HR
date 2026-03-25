using System.Diagnostics.Metrics;
using System.Text.Json;
using LeaveManagement.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LeaveManagement.Infrastructure.Caching;

public sealed class RedisAppCache : IAppCache
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly Meter CacheMeter = new("LeaveManagement.Cache", "1.0.0");
    private static readonly Counter<long> CacheHitCounter = CacheMeter.CreateCounter<long>("cache_hit_total");
    private static readonly Counter<long> CacheMissCounter = CacheMeter.CreateCounter<long>("cache_miss_total");
    private static readonly Counter<long> CacheSetCounter = CacheMeter.CreateCounter<long>("cache_set_total");
    private static readonly Counter<long> CacheRemoveCounter = CacheMeter.CreateCounter<long>("cache_remove_total");

    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisAppCache> _logger;
    private readonly RedisCacheOptions _options;

    public RedisAppCache(
        IDistributedCache cache,
        ILogger<RedisAppCache> logger,
        IOptions<RedisCacheOptions> options)
    {
        _cache = cache;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        var payload = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrWhiteSpace(payload))
        {
            CacheMissCounter.Add(1);
            _logger.LogDebug("cache_miss key={Key}", key);
            return default;
        }

        CacheHitCounter.Add(1);
        _logger.LogDebug("cache_hit key={Key}", key);
        return JsonSerializer.Deserialize<T>(payload, JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken cancellationToken)
    {
        var effectiveTtl = ttl ?? TimeSpan.FromSeconds(Math.Max(1, _options.DefaultTtlSeconds));
        var payload = JsonSerializer.Serialize(value, JsonOptions);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = effectiveTtl
        };

        await _cache.SetStringAsync(key, payload, cacheOptions, cancellationToken);
        CacheSetCounter.Add(1);
        _logger.LogDebug("cache_set key={Key} ttlSeconds={Ttl}", key, (int)effectiveTtl.TotalSeconds);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(key, cancellationToken);
        CacheRemoveCounter.Add(1);
        _logger.LogDebug("cache_remove key={Key}", key);
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken)
    {
        // IDistributedCache does not expose key scanning; use explicit key invalidation in services.
        _logger.LogWarning("cache_remove_prefix_not_supported prefix={Prefix}", prefix);
        return Task.CompletedTask;
    }
}
