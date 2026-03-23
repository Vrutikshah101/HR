using System.Text;
using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Reports;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _dbContext;

    public ReportService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ReportRowDto>> GetLeaveBalanceAsync(ReportFilters filters, CancellationToken cancellationToken)
    {
        var query = from lb in _dbContext.LeaveBalances.AsNoTracking()
                    join e in _dbContext.Employees.AsNoTracking() on lb.EmployeeId equals e.Id
                    select new
                    {
                        e.EmployeeCode,
                        e.FullName,
                        e.Department,
                        lb.LeaveTypeCode,
                        lb.OpeningBalance,
                        lb.Used,
                        lb.Adjustments
                    };

        if (!string.IsNullOrWhiteSpace(filters.Department))
        {
            var dept = filters.Department.Trim();
            query = query.Where(x => x.Department == dept);
        }

        if (!string.IsNullOrWhiteSpace(filters.LeaveTypeCode))
        {
            var type = filters.LeaveTypeCode.Trim().ToUpperInvariant();
            query = query.Where(x => x.LeaveTypeCode == type);
        }

        var data = await query.OrderBy(x => x.EmployeeCode).ThenBy(x => x.LeaveTypeCode).ToListAsync(cancellationToken);

        return data.Select(x => new ReportRowDto(new Dictionary<string, object?>
        {
            ["employeeCode"] = x.EmployeeCode,
            ["employeeName"] = x.FullName,
            ["department"] = x.Department,
            ["leaveType"] = x.LeaveTypeCode,
            ["opening"] = x.OpeningBalance,
            ["used"] = x.Used,
            ["adjustments"] = x.Adjustments,
            ["available"] = x.OpeningBalance + x.Adjustments - x.Used
        })).ToArray();
    }

    public async Task<IReadOnlyCollection<ReportRowDto>> GetDepartmentSummaryAsync(ReportFilters filters, CancellationToken cancellationToken)
    {
        var from = filters.DateFrom ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
        var to = filters.DateTo ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var data = await _dbContext.LeaveRequests
            .AsNoTracking()
            .Where(x => x.StartDate >= from && x.StartDate <= to)
            .Join(_dbContext.Employees.AsNoTracking(), lr => lr.EmployeeId, e => e.Id, (lr, e) => new { lr, e })
            .GroupBy(x => x.e.Department)
            .Select(g => new
            {
                Department = g.Key,
                Total = g.Count(),
                Approved = g.Count(x => x.lr.Status == LeaveRequestStatus.Approved),
                Rejected = g.Count(x => x.lr.Status == LeaveRequestStatus.Rejected),
                Pending = g.Count(x => x.lr.Status == LeaveRequestStatus.PendingLevel1 || x.lr.Status == LeaveRequestStatus.PendingLevel2)
            })
            .OrderBy(x => x.Department)
            .ToListAsync(cancellationToken);

        return data.Select(x => new ReportRowDto(new Dictionary<string, object?>
        {
            ["department"] = x.Department,
            ["totalRequests"] = x.Total,
            ["approved"] = x.Approved,
            ["rejected"] = x.Rejected,
            ["pending"] = x.Pending
        })).ToArray();
    }

    public async Task<IReadOnlyCollection<ReportRowDto>> GetMonthlyUtilizationAsync(ReportFilters filters, CancellationToken cancellationToken)
    {
        var year = filters.Year ?? DateTime.UtcNow.Year;

        var data = await _dbContext.LeaveRequests
            .AsNoTracking()
            .Where(x => x.StartDate.Year == year && x.Status == LeaveRequestStatus.Approved)
            .GroupBy(x => x.StartDate.Month)
            .Select(g => new
            {
                Month = g.Key,
                ApprovedRequests = g.Count(),
                ApprovedDays = g.Sum(x => x.Days)
            })
            .OrderBy(x => x.Month)
            .ToListAsync(cancellationToken);

        return data.Select(x => new ReportRowDto(new Dictionary<string, object?>
        {
            ["year"] = year,
            ["month"] = x.Month,
            ["approvedRequests"] = x.ApprovedRequests,
            ["approvedDays"] = x.ApprovedDays
        })).ToArray();
    }

    public async Task<IReadOnlyCollection<ReportRowDto>> GetApprovalSummaryAsync(ReportFilters filters, CancellationToken cancellationToken)
    {
        var from = filters.DateFrom?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow.AddMonths(-1);
        var to = filters.DateTo?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.UtcNow;

        var data = await _dbContext.LeaveRequestApprovals
            .AsNoTracking()
            .Where(x => x.ActionedAtUtc >= from && x.ActionedAtUtc <= to)
            .GroupBy(x => new { x.ApprovalLevel, x.Action })
            .Select(g => new
            {
                g.Key.ApprovalLevel,
                g.Key.Action,
                Count = g.Count()
            })
            .OrderBy(x => x.ApprovalLevel)
            .ToListAsync(cancellationToken);

        return data.Select(x => new ReportRowDto(new Dictionary<string, object?>
        {
            ["approvalLevel"] = x.ApprovalLevel,
            ["action"] = x.Action,
            ["count"] = x.Count
        })).ToArray();
    }

    public byte[] ToCsv(IReadOnlyCollection<ReportRowDto> rows)
    {
        if (rows.Count == 0)
        {
            return Encoding.UTF8.GetBytes(string.Empty);
        }

        var headers = rows.SelectMany(x => x.Fields.Keys).Distinct().ToArray();
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", headers));

        foreach (var row in rows)
        {
            var cells = headers.Select(header => EscapeCsv(row.Fields.TryGetValue(header, out var value) ? value : null));
            sb.AppendLine(string.Join(",", cells));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string EscapeCsv(object? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        var text = value.ToString() ?? string.Empty;
        if (text.Contains(',') || text.Contains('"') || text.Contains('\n'))
        {
            text = $"\"{text.Replace("\"", "\"\"")}\"";
        }

        return text;
    }
}
