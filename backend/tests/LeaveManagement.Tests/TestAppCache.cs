using LeaveManagement.Application.Abstractions;

namespace LeaveManagement.Tests;

internal sealed class TestAppCache : IAppCache
{
    private readonly Dictionary<string, object?> _store = new(StringComparer.Ordinal);

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        if (_store.TryGetValue(key, out var value) && value is T typed)
        {
            return Task.FromResult<T?>(typed);
        }

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken cancellationToken)
    {
        _store[key] = value;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        _store.Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken)
    {
        var keys = _store.Keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToArray();
        foreach (var key in keys)
        {
            _store.Remove(key);
        }

        return Task.CompletedTask;
    }
}
