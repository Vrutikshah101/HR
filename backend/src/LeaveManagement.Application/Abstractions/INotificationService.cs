namespace LeaveManagement.Application.Abstractions;

public interface INotificationService
{
    Task NotifyLeaveAppliedAsync(Guid leaveRequestId, Guid employeeId, Guid? level1ApproverEmployeeId, Guid? level2ApproverEmployeeId, CancellationToken cancellationToken);
    Task NotifyLeaveActionedAsync(Guid leaveRequestId, Guid employeeId, string action, int approvalLevel, CancellationToken cancellationToken);
    Task NotifyLeaveCancelledAsync(Guid leaveRequestId, Guid employeeId, CancellationToken cancellationToken);
}
