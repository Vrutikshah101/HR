using LeaveManagement.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Infrastructure.Services;

public class LoggingNotificationService : INotificationService
{
    private readonly ILogger<LoggingNotificationService> _logger;

    public LoggingNotificationService(ILogger<LoggingNotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyLeaveAppliedAsync(Guid leaveRequestId, Guid employeeId, Guid? level1ApproverEmployeeId, Guid? level2ApproverEmployeeId, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Notification: leave applied. LeaveRequestId={LeaveRequestId} EmployeeId={EmployeeId} L1={Level1ApproverEmployeeId} L2={Level2ApproverEmployeeId}",
            leaveRequestId,
            employeeId,
            level1ApproverEmployeeId,
            level2ApproverEmployeeId);

        return Task.CompletedTask;
    }

    public Task NotifyLeaveActionedAsync(Guid leaveRequestId, Guid employeeId, string action, int approvalLevel, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Notification: leave actioned. LeaveRequestId={LeaveRequestId} EmployeeId={EmployeeId} Action={Action} Level={ApprovalLevel}",
            leaveRequestId,
            employeeId,
            action,
            approvalLevel);

        return Task.CompletedTask;
    }

    public Task NotifyLeaveCancelledAsync(Guid leaveRequestId, Guid employeeId, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Notification: leave cancelled. LeaveRequestId={LeaveRequestId} EmployeeId={EmployeeId}",
            leaveRequestId,
            employeeId);

        return Task.CompletedTask;
    }
}
