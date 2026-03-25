using LeaveManagement.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LeaveManagement.Tests;

public class RedisAppCacheTests
{
    private static RedisAppCache CreateSut()
    {
        var services = new ServiceCollection();
        services.AddDistributedMemoryCache();
        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<IDistributedCache>();
        var options = Options.Create(new RedisCacheOptions
        {
            Enabled = false,
            ConnectionString = string.Empty,
            DefaultTtlSeconds = 60,
            InstanceName = "lms:"
        });

        return new RedisAppCache(cache, NullLogger<RedisAppCache>.Instance, options);
    }

    [Fact]
    public async Task SetAndGet_ShouldRoundtripValue()
    {
        var sut = CreateSut();
        var key = "test:key:1";

        await sut.SetAsync(key, new SamplePayload("abc"), TimeSpan.FromMinutes(1), CancellationToken.None);
        var result = await sut.GetAsync<SamplePayload>(key, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("abc", result!.Value);
    }

    [Fact]
    public async Task Remove_ShouldDeleteValue()
    {
        var sut = CreateSut();
        var key = "test:key:2";

        await sut.SetAsync(key, new SamplePayload("xyz"), TimeSpan.FromMinutes(1), CancellationToken.None);
        await sut.RemoveAsync(key, CancellationToken.None);
        var result = await sut.GetAsync<SamplePayload>(key, CancellationToken.None);

        Assert.Null(result);
    }

    private sealed record SamplePayload(string Value);
}
