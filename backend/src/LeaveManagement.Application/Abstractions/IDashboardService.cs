using LeaveManagement.Application.Dashboard;

namespace LeaveManagement.Application.Abstractions;

public interface IDashboardService
{
    Task<IReadOnlyCollection<DashboardCardDto>> GetEmployeeCardsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DashboardCardDto>> GetManagerCardsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DashboardCardDto>> GetHrCardsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DashboardCardDto>> GetAdminCardsAsync(CancellationToken cancellationToken);
}
