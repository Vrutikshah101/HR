using LeaveManagement.Application.Leaves;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Services;

public class LeaveWorkflowService : ILeaveWorkflowService
{
    private readonly ApplicationDbContext _dbContext;

    public LeaveWorkflowService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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

        if (action == "Approved")
        {
            request.Status = request.Status == LeaveRequestStatus.PendingLevel1 && hierarchy.Level2ApproverEmployeeId.HasValue
                ? LeaveRequestStatus.PendingLevel2
                : LeaveRequestStatus.Approved;
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

        return request;
    }
}
