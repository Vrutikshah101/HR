namespace LeaveManagement.Infrastructure.Caching;

public static class CacheTtls
{
    public static readonly TimeSpan Masters = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan Dashboard = TimeSpan.FromMinutes(2);
    public static readonly TimeSpan Hierarchy = TimeSpan.FromMinutes(10);
    public static readonly TimeSpan LeaveBalance = TimeSpan.FromMinutes(2);
    public static readonly TimeSpan UserProfile = TimeSpan.FromMinutes(10);
}
