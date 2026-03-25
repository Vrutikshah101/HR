using LeaveManagement.Infrastructure.Persistence;
using LeaveManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LeaveManagement.Tests;

public class LeaveBalanceServiceTests
{
    [Fact]
    public void CalculateWorkingDays_Should_Exclude_Weekend_And_Holidays()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new ApplicationDbContext(options);
        var leaveOptions = Options.Create(new LeaveOptions
        {
            Types =
            [
                new LeaveTypeOption { Code = "CL", Name = "Casual", DefaultOpeningBalance = 10 }
            ],
            Holidays =
            [
                new HolidayOption { Date = new DateOnly(2026, 1, 26), Name = "Holiday" }
            ]
        });

        var service = new LeaveBalanceService(dbContext, leaveOptions, new TestAppCache());

        var days = service.CalculateWorkingDays(new DateOnly(2026, 1, 23), new DateOnly(2026, 1, 27));

        // Fri + Tue = 2 (Sat/Sun + holiday Monday excluded)
        Assert.Equal(2m, days);
    }
}
