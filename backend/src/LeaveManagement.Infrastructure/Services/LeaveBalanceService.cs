using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Balances;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LeaveManagement.Infrastructure.Services;

public class LeaveBalanceService : ILeaveBalanceService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly LeaveOptions _leaveOptions;

    public LeaveBalanceService(ApplicationDbContext dbContext, IOptions<LeaveOptions> leaveOptions)
    {
        _dbContext = dbContext;
        _leaveOptions = leaveOptions.Value;
    }

    public decimal CalculateWorkingDays(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
        {
            return 0;
        }

        var holidays = GetHolidaySet();
        var total = 0m;

        for (var day = startDate; day <= endDate; day = day.AddDays(1))
        {
            var dayOfWeek = day.DayOfWeek;
            if (dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            if (holidays.Contains(day))
            {
                continue;
            }

            total += 1;
        }

        return total;
    }

    public async Task ValidateSufficientBalanceAsync(Guid employeeId, string leaveTypeCode, decimal requestedDays, CancellationToken cancellationToken)
    {
        var balance = await GetOrCreateBalanceAsync(employeeId, leaveTypeCode, cancellationToken);

        if (balance.Available < requestedDays)
        {
            throw new InvalidOperationException($"Insufficient {leaveTypeCode} balance. Available: {balance.Available}.");
        }
    }

    public async Task ApplyApprovedDeductionAsync(LeaveRequest request, CancellationToken cancellationToken)
    {
        var balance = await GetOrCreateBalanceAsync(request.EmployeeId, request.LeaveTypeCode, cancellationToken);
        balance.Used += request.Days;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreForCancellationAsync(LeaveRequest request, CancellationToken cancellationToken)
    {
        var balance = await GetOrCreateBalanceAsync(request.EmployeeId, request.LeaveTypeCode, cancellationToken);
        balance.Used -= request.Days;

        if (balance.Used < 0)
        {
            balance.Used = 0;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LeaveBalanceSummaryDto>> GetMyBalancesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employeeId = await _dbContext.Employees
            .Where(x => x.UserId == userId)
            .Select(x => x.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (employeeId == Guid.Empty)
        {
            throw new InvalidOperationException("Employee profile not found for current user.");
        }

        var list = await _dbContext.LeaveBalances
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderBy(x => x.LeaveTypeCode)
            .Select(x => new LeaveBalanceSummaryDto(x.LeaveTypeCode, x.OpeningBalance, x.Used, x.Adjustments, x.OpeningBalance + x.Adjustments - x.Used))
            .ToArrayAsync(cancellationToken);

        return list;
    }

    public Task<IReadOnlyCollection<DateOnly>> GetHolidaysAsync(CancellationToken cancellationToken)
    {
        return GetHolidaysInternalAsync(cancellationToken);
    }

    private async Task<IReadOnlyCollection<DateOnly>> GetHolidaysInternalAsync(CancellationToken cancellationToken)
    {
        var dbHolidays = await _dbContext.Holidays.AsNoTracking().OrderBy(x => x.Date).Select(x => x.Date).ToArrayAsync(cancellationToken);
        if (dbHolidays.Length > 0)
        {
            return dbHolidays;
        }

        return _leaveOptions.Holidays
            .Select(x => x.Date)
            .OrderBy(x => x)
            .ToArray();
    }

    private HashSet<DateOnly> GetHolidaySet()
    {
        var dbHolidays = _dbContext.Holidays.AsNoTracking().Select(x => x.Date).ToList();
        if (dbHolidays.Count > 0)
        {
            return dbHolidays.ToHashSet();
        }

        return _leaveOptions.Holidays.Select(x => x.Date).ToHashSet();
    }

    private async Task<LeaveBalance> GetOrCreateBalanceAsync(Guid employeeId, string leaveTypeCode, CancellationToken cancellationToken)
    {
        var code = leaveTypeCode.Trim().ToUpperInvariant();

        var balance = await _dbContext.LeaveBalances
            .SingleOrDefaultAsync(x => x.EmployeeId == employeeId && x.LeaveTypeCode == code, cancellationToken);

        if (balance is not null)
        {
            return balance;
        }

        var leaveType = _leaveOptions.Types.FirstOrDefault(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        var opening = leaveType?.DefaultOpeningBalance ?? 0;

        balance = new LeaveBalance
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            LeaveTypeCode = code,
            OpeningBalance = opening,
            Used = 0,
            Adjustments = 0
        };

        await _dbContext.LeaveBalances.AddAsync(balance, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return balance;
    }
}
