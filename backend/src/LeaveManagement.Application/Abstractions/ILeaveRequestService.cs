using LeaveManagement.Application.Leaves;
using LeaveManagement.Domain.Entities;

namespace LeaveManagement.Application.Abstractions;

public interface ILeaveRequestService
{
    Task<IReadOnlyCollection<LeaveTypeDto>> GetLeaveTypesAsync(CancellationToken cancellationToken);
    Task<LeaveRequest> ApplyAsync(Guid userId, ApplyLeaveCommand command, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LeaveRequest>> GetMyAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LeaveRequest>> GetTeamPendingAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyDictionary<Guid, EmployeeIdentityDto>> GetEmployeeIdentitiesAsync(IReadOnlyCollection<Guid> employeeIds, CancellationToken cancellationToken);
    Task<LeaveRequest?> GetByIdAsync(Guid userId, IReadOnlyCollection<string> roles, Guid requestId, CancellationToken cancellationToken);
    Task<LeaveRequest> CancelAsync(Guid userId, Guid requestId, CancellationToken cancellationToken);
}
