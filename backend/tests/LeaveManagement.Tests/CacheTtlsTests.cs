using LeaveManagement.Infrastructure.Caching;

namespace LeaveManagement.Tests;

public class CacheTtlsTests
{
    [Fact]
    public void MastersTtl_ShouldBeGreaterThanDashboardTtl()
    {
        Assert.True(CacheTtls.Masters > CacheTtls.Dashboard);
    }

    [Fact]
    public void UserProfileTtl_ShouldBeExpectedMinutes()
    {
        Assert.Equal(TimeSpan.FromMinutes(10), CacheTtls.UserProfile);
    }
}
