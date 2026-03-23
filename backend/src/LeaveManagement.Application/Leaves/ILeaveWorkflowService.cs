using LeaveManagement.Domain.Entities;

namespace LeaveManagement.Application.Leaves;

public interface ILeaveWorkflowService
{
    Task<LeaveRequest> ApproveAsync(Guid actorUserId, Guid requestId, string? comment, CancellationToken cancellationToken);
    Task<LeaveRequest> RejectAsync(Guid actorUserId, Guid requestId, string? comment, CancellationToken cancellationToken);
}
