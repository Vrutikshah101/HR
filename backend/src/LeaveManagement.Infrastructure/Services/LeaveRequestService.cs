using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Leaves;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LeaveManagement.Infrastructure.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private static readonly LeaveRequestStatus[] ActiveStatuses =
    [
        LeaveRequestStatus.PendingLevel1,
        LeaveRequestStatus.PendingLevel2,
        LeaveRequestStatus.Approved
    ];

    private readonly ApplicationDbContext _dbContext;
    private readonly LeaveOptions _leaveOptions;

    public LeaveRequestService(ApplicationDbContext dbContext, IOptions<LeaveOptions> leaveOptions)
    {
        _dbContext = dbContext;
        _leaveOptions = leaveOptions.Value;
    }

    public Task<IReadOnlyCollection<LeaveTypeDto>> GetLeaveTypesAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<LeaveTypeDto> result = _leaveOptions.Types
            .Where(x => !string.IsNullOrWhiteSpace(x.Code) && !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new LeaveTypeDto(x.Code.Trim().ToUpperInvariant(), x.Name.Trim()))
            .DistinctBy(x => x.Code)
            .OrderBy(x => x.Code)
            .ToArray();

        return Task.FromResult(result);
    }

    public async Task<LeaveRequest> ApplyAsync(Guid userId, ApplyLeaveCommand command, CancellationToken cancellationToken)
    {
        var employeeId = await ResolveEmployeeIdAsync(userId, cancellationToken);
        ValidateApplyCommand(command);

        var leaveTypes = await GetLeaveTypesAsync(cancellationToken);
        if (!leaveTypes.Any(x => x.Code.Equals(command.LeaveTypeCode, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Invalid leave type code.");
        }

        var hasOverlap = await _dbContext.LeaveRequests.AnyAsync(x =>
            x.EmployeeId == employeeId
            && ActiveStatuses.Contains(x.Status)
            && x.StartDate <= command.EndDate
            && x.EndDate >= command.StartDate,
            cancellationToken);

        if (hasOverlap)
        {
            throw new InvalidOperationException("Leave request overlaps with an existing active leave request.");
        }

        var hierarchy = await _dbContext.ReportingHierarchies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.EmployeeId == employeeId, cancellationToken);

        if (hierarchy is null || (hierarchy.Level1ApproverEmployeeId is null && hierarchy.Level2ApproverEmployeeId is null))
        {
            throw new InvalidOperationException("No approver is configured for this employee.");
        }

        var status = hierarchy.Level1ApproverEmployeeId is null
            ? LeaveRequestStatus.PendingLevel2
            : LeaveRequestStatus.PendingLevel1;

        var request = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            LeaveTypeCode = command.LeaveTypeCode.Trim().ToUpperInvariant(),
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Days = command.Days,
            Reason = command.Reason.Trim(),
            Status = status,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await _dbContext.LeaveRequests.AddAsync(request, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<IReadOnlyCollection<LeaveRequest>> GetMyAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employeeId = await ResolveEmployeeIdAsync(userId, cancellationToken);

        return await _dbContext.LeaveRequests
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LeaveRequest>> GetTeamPendingAsync(Guid userId, CancellationToken cancellationToken)
    {
        var approverEmployeeId = await ResolveEmployeeIdAsync(userId, cancellationToken);

        var subordinateToL1 = _dbContext.ReportingHierarchies
            .Where(x => x.Level1ApproverEmployeeId == approverEmployeeId)
            .Select(x => x.EmployeeId);

        var subordinateToL2 = _dbContext.ReportingHierarchies
            .Where(x => x.Level2ApproverEmployeeId == approverEmployeeId)
            .Select(x => x.EmployeeId);

        var l1Pending = _dbContext.LeaveRequests
            .Where(x => subordinateToL1.Contains(x.EmployeeId) && x.Status == LeaveRequestStatus.PendingLevel1);

        var l2Pending = _dbContext.LeaveRequests
            .Where(x => subordinateToL2.Contains(x.EmployeeId) && x.Status == LeaveRequestStatus.PendingLevel2);

        return await l1Pending
            .Union(l2Pending)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<LeaveRequest?> GetByIdAsync(Guid userId, IReadOnlyCollection<string> roles, Guid requestId, CancellationToken cancellationToken)
    {
        var employeeId = await ResolveEmployeeIdAsync(userId, cancellationToken);

        var request = await _dbContext.LeaveRequests
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == requestId, cancellationToken);

        if (request is null)
        {
            return null;
        }

        if (roles.Any(role => role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                           || role.Equals("Hr", StringComparison.OrdinalIgnoreCase)))
        {
            return request;
        }

        if (request.EmployeeId == employeeId)
        {
            return request;
        }

        var hierarchy = await _dbContext.ReportingHierarchies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.EmployeeId == request.EmployeeId, cancellationToken);

        if (hierarchy is null)
        {
            return null;
        }

        if (hierarchy.Level1ApproverEmployeeId == employeeId || hierarchy.Level2ApproverEmployeeId == employeeId)
        {
            return request;
        }

        return null;
    }

    public async Task<LeaveRequest> CancelAsync(Guid userId, Guid requestId, CancellationToken cancellationToken)
    {
        var employeeId = await ResolveEmployeeIdAsync(userId, cancellationToken);

        var request = await _dbContext.LeaveRequests
            .SingleOrDefaultAsync(x => x.Id == requestId && x.EmployeeId == employeeId, cancellationToken);

        if (request is null)
        {
            throw new InvalidOperationException("Leave request not found.");
        }

        if (request.Status is not LeaveRequestStatus.PendingLevel1 and not LeaveRequestStatus.PendingLevel2)
        {
            throw new InvalidOperationException("Only pending leave requests can be cancelled.");
        }

        request.Status = LeaveRequestStatus.Cancelled;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return request;
    }

    private async Task<Guid> ResolveEmployeeIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employeeId = await _dbContext.Employees
            .Where(x => x.UserId == userId)
            .Select(x => x.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (employeeId == Guid.Empty)
        {
            throw new InvalidOperationException("Employee profile not found for current user.");
        }

        return employeeId;
    }

    private static void ValidateApplyCommand(ApplyLeaveCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.LeaveTypeCode))
        {
            throw new InvalidOperationException("Leave type code is required.");
        }

        if (command.StartDate > command.EndDate)
        {
            throw new InvalidOperationException("Start date cannot be after end date.");
        }

        if (command.Days <= 0)
        {
            throw new InvalidOperationException("Days must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new InvalidOperationException("Reason is required.");
        }

        var maxDays = (decimal)(command.EndDate.DayNumber - command.StartDate.DayNumber + 1);
        if (command.Days > maxDays)
        {
            throw new InvalidOperationException("Days cannot exceed inclusive day span between start and end dates.");
        }
    }
}

