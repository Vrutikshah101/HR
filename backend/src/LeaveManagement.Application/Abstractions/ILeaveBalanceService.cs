using LeaveManagement.Application.Balances;
using LeaveManagement.Domain.Entities;

namespace LeaveManagement.Application.Abstractions;

public interface ILeaveBalanceService
{
    decimal CalculateWorkingDays(DateOnly startDate, DateOnly endDate);
    Task ValidateSufficientBalanceAsync(Guid employeeId, string leaveTypeCode, decimal requestedDays, CancellationToken cancellationToken);
    Task ApplyApprovedDeductionAsync(LeaveRequest request, CancellationToken cancellationToken);
    Task RestoreForCancellationAsync(LeaveRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LeaveBalanceSummaryDto>> GetMyBalancesAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DateOnly>> GetHolidaysAsync(CancellationToken cancellationToken);
}
