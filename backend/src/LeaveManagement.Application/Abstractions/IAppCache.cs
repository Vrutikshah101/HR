namespace LeaveManagement.Application.Abstractions;

public interface IAppCache
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl, CancellationToken cancellationToken);
    Task RemoveAsync(string key, CancellationToken cancellationToken);
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken);
}
