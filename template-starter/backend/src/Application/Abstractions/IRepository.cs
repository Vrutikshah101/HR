namespace Application.Abstractions;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(T entity, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
