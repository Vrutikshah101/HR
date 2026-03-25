using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Leaves;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Infrastructure.Caching;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Infrastructure.Services;

public class LeaveWorkflowService : ILeaveWorkflowService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILeaveBalanceService _leaveBalanceService;
    private readonly INotificationService _notificationService;
    private readonly IAppCache _appCache;
    private readonly ILogger<LeaveWorkflowService> _logger;

    public LeaveWorkflowService(
        ApplicationDbContext dbContext,
        ILeaveBalanceService leaveBalanceService,
        INotificationService notificationService,
        IAppCache appCache,
        ILogger<LeaveWorkflowService> logger)
    {
        _dbContext = dbContext;
        _leaveBalanceService = leaveBalanceService;
        _notificationService = notificationService;
        _appCache = appCache;
        _logger = logger;
    }

    public Task<LeaveRequest> ApproveAsync(Guid actorUserId, Guid requestId, string? comment, CancellationToken cancellationToken)
    {
        return HandleActionAsync(actorUserId, requestId, "Approved", comment, cancellationToken);
    }

    public Task<LeaveRequest> RejectAsync(Guid actorUserId, Guid requestId, string? comment, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException("Reject comment is required.");
        }

        return HandleActionAsync(actorUserId, requestId, "Rejected", comment, cancellationToken);
    }

    private async Task<LeaveRequest> HandleActionAsync(
        Guid actorUserId,
        Guid requestId,
        string action,
        string? comment,
        CancellationToken cancellationToken)
    {
        var approverEmployeeId = await _dbContext.Employees
            .Where(x => x.UserId == actorUserId)
            .Select(x => x.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (approverEmployeeId == Guid.Empty)
        {
            throw new InvalidOperationException("Current user does not have an employee profile.");
        }

        var request = await _dbContext.LeaveRequests
            .SingleOrDefaultAsync(x => x.Id == requestId, cancellationToken);

        if (request is null)
        {
            throw new InvalidOperationException("Leave request not found.");
        }

        var hierarchy = await _dbContext.ReportingHierarchies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.EmployeeId == request.EmployeeId, cancellationToken);

        if (hierarchy is null)
        {
            throw new InvalidOperationException("Reporting hierarchy is not configured for request employee.");
        }

        var approvalLevel = request.Status switch
        {
            LeaveRequestStatus.PendingLevel1 => 1,
            LeaveRequestStatus.PendingLevel2 => 2,
            _ => 0
        };

        if (approvalLevel == 0)
        {
            throw new InvalidOperationException("Only pending leave requests can be actioned.");
        }

        var expectedApprover = approvalLevel == 1
            ? hierarchy.Level1ApproverEmployeeId
            : hierarchy.Level2ApproverEmployeeId;

        if (expectedApprover is null)
        {
            throw new InvalidOperationException($"Approver for level {approvalLevel} is not configured.");
        }

        if (expectedApprover.Value != approverEmployeeId)
        {
            throw new InvalidOperationException($"Current user is not authorized for approval level {approvalLevel}.");
        }

        var finalApprovalReached = false;

        if (action == "Approved")
        {
            request.Status = request.Status == LeaveRequestStatus.PendingLevel1 && hierarchy.Level2ApproverEmployeeId.HasValue
                ? LeaveRequestStatus.PendingLevel2
                : LeaveRequestStatus.Approved;

            finalApprovalReached = request.Status == LeaveRequestStatus.Approved;
        }
        else
        {
            request.Status = LeaveRequestStatus.Rejected;
        }

        var audit = new LeaveRequestApproval
        {
            Id = Guid.NewGuid(),
            LeaveRequestId = request.Id,
            ApprovalLevel = approvalLevel,
            ApproverEmployeeId = approverEmployeeId,
            Action = action,
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
            ActionedAtUtc = DateTimeOffset.UtcNow
        };

        await _dbContext.LeaveRequestApprovals.AddAsync(audit, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (finalApprovalReached)
        {
            await _leaveBalanceService.ApplyApprovedDeductionAsync(request, cancellationToken);
        }

        _logger.LogInformation(
            "Leave action completed. RequestId={RequestId} Action={Action} Level={ApprovalLevel} NewStatus={Status}",
            request.Id,
            action,
            approvalLevel,
            request.Status);

        await _notificationService.NotifyLeaveActionedAsync(request.Id, request.EmployeeId, action, approvalLevel, cancellationToken);
        await InvalidateWorkflowCachesAsync(request.EmployeeId, approverEmployeeId, hierarchy.Level1ApproverEmployeeId, hierarchy.Level2ApproverEmployeeId, cancellationToken);

        return request;
    }

    private async Task InvalidateWorkflowCachesAsync(
        Guid requestEmployeeId,
        Guid actorApproverEmployeeId,
        Guid? level1ApproverEmployeeId,
        Guid? level2ApproverEmployeeId,
        CancellationToken cancellationToken)
    {
        await _appCache.RemoveAsync(CacheKeys.LeaveBalances(requestEmployeeId), cancellationToken);
        await _appCache.RemoveAsync(CacheKeys.EmployeeDashboard(requestEmployeeId), cancellationToken);

        var managerIds = new[]
        {
            actorApproverEmployeeId,
            level1ApproverEmployeeId ?? Guid.Empty,
            level2ApproverEmployeeId ?? Guid.Empty
        }
        .Where(x => x != Guid.Empty)
        .Distinct();

        foreach (var managerId in managerIds)
        {
            await _appCache.RemoveAsync(CacheKeys.ManagerDashboard(managerId), cancellationToken);
        }

        await _appCache.RemoveAsync(CacheKeys.HrDashboard(), cancellationToken);
        await _appCache.RemoveAsync(CacheKeys.AdminDashboard(), cancellationToken);
    }
}
