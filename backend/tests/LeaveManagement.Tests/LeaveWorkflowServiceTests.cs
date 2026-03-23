using LeaveManagement.Infrastructure.Persistence;
using LeaveManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LeaveManagement.Tests;

public class LeaveWorkflowServiceTests
{
    [Fact]
    public async Task Reject_Without_Comment_Should_Throw()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new ApplicationDbContext(options);
        var balanceService = new LeaveBalanceService(dbContext, Options.Create(new LeaveOptions()));
        var notificationService = new LoggingNotificationService(new NullLogger<LoggingNotificationService>());
        var workflow = new LeaveWorkflowService(dbContext, balanceService, notificationService, new NullLogger<LeaveWorkflowService>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => workflow.RejectAsync(Guid.NewGuid(), Guid.NewGuid(), "", CancellationToken.None));
    }
}
