using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Dashboard;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _dbContext;

    public DashboardService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<DashboardCardDto>> GetEmployeeCardsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employeeId = await ResolveEmployeeIdAsync(userId, cancellationToken);

        var available = await _dbContext.LeaveBalances
            .Where(x => x.EmployeeId == employeeId)
            .SumAsync(x => x.OpeningBalance + x.Adjustments - x.Used, cancellationToken);

        var pending = await _dbContext.LeaveRequests
            .CountAsync(x => x.EmployeeId == employeeId && (x.Status == LeaveRequestStatus.PendingLevel1 || x.Status == LeaveRequestStatus.PendingLevel2), cancellationToken);

        var approvedThisYear = await _dbContext.LeaveRequests
            .CountAsync(x => x.EmployeeId == employeeId && x.Status == LeaveRequestStatus.Approved && x.StartDate.Year == DateTime.UtcNow.Year, cancellationToken);

        return
        [
            new("availableLeave", "Available Leave", available),
            new("pendingRequests", "Pending Requests", pending),
            new("approvedThisYear", "Approved This Year", approvedThisYear)
        ];
    }

    public async Task<IReadOnlyCollection<DashboardCardDto>> GetManagerCardsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var approverEmployeeId = await ResolveEmployeeIdAsync(userId, cancellationToken);

        var subordinateL1 = _dbContext.ReportingHierarchies.Where(x => x.Level1ApproverEmployeeId == approverEmployeeId).Select(x => x.EmployeeId);
        var subordinateL2 = _dbContext.ReportingHierarchies.Where(x => x.Level2ApproverEmployeeId == approverEmployeeId).Select(x => x.EmployeeId);

        var pendingL1 = await _dbContext.LeaveRequests.CountAsync(x => subordinateL1.Contains(x.EmployeeId) && x.Status == LeaveRequestStatus.PendingLevel1, cancellationToken);
        var pendingL2 = await _dbContext.LeaveRequests.CountAsync(x => subordinateL2.Contains(x.EmployeeId) && x.Status == LeaveRequestStatus.PendingLevel2, cancellationToken);
        var totalTeam = await _dbContext.ReportingHierarchies.CountAsync(x => x.Level1ApproverEmployeeId == approverEmployeeId || x.Level2ApproverEmployeeId == approverEmployeeId, cancellationToken);

        return
        [
            new("pendingLevel1", "Pending Level 1", pendingL1),
            new("pendingLevel2", "Pending Level 2", pendingL2),
            new("totalTeam", "Team Members", totalTeam)
        ];
    }

    public async Task<IReadOnlyCollection<DashboardCardDto>> GetHrCardsAsync(CancellationToken cancellationToken)
    {
        var open = await _dbContext.LeaveRequests.CountAsync(x => x.Status == LeaveRequestStatus.PendingLevel1 || x.Status == LeaveRequestStatus.PendingLevel2, cancellationToken);
        var approvedMonth = await _dbContext.LeaveRequests.CountAsync(x => x.Status == LeaveRequestStatus.Approved && x.StartDate.Month == DateTime.UtcNow.Month && x.StartDate.Year == DateTime.UtcNow.Year, cancellationToken);
        var rejectedMonth = await _dbContext.LeaveRequests.CountAsync(x => x.Status == LeaveRequestStatus.Rejected && x.CreatedAtUtc.Month == DateTime.UtcNow.Month && x.CreatedAtUtc.Year == DateTime.UtcNow.Year, cancellationToken);

        return
        [
            new("openRequests", "Open Requests", open),
            new("approvedMonth", "Approved This Month", approvedMonth),
            new("rejectedMonth", "Rejected This Month", rejectedMonth)
        ];
    }

    public async Task<IReadOnlyCollection<DashboardCardDto>> GetAdminCardsAsync(CancellationToken cancellationToken)
    {
        var activeUsers = await _dbContext.Users.CountAsync(x => x.IsActive, cancellationToken);
        var employees = await _dbContext.Employees.CountAsync(cancellationToken);
        var approvals = await _dbContext.LeaveRequestApprovals.CountAsync(cancellationToken);

        return
        [
            new("activeUsers", "Active Users", activeUsers),
            new("employees", "Employees", employees),
            new("approvalActions", "Approval Actions", approvals)
        ];
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
}
