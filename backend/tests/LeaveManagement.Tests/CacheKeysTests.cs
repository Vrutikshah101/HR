using LeaveManagement.Infrastructure.Caching;

namespace LeaveManagement.Tests;

public class CacheKeysTests
{
    [Fact]
    public void UserProfile_ShouldUseNormalizedKeyFormat()
    {
        var id = Guid.Parse("11111111-2222-3333-4444-555555555555");

        var key = CacheKeys.UserProfile(id);

        Assert.Equal("lms:user:profile:11111111222233334444555555555555", key);
    }

    [Fact]
    public void EmployeeDashboard_ShouldUseEmployeeScope()
    {
        var id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

        var key = CacheKeys.EmployeeDashboard(id);

        Assert.Equal("lms:dashboard:employee:aaaaaaaabbbbccccddddeeeeeeeeeeee", key);
    }
}
