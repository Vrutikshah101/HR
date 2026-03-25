namespace LeaveManagement.Infrastructure.Caching;

public sealed class RedisCacheOptions
{
    public const string SectionName = "Redis";

    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = "localhost:6379";
    public string InstanceName { get; set; } = "lms:";
    public int DefaultTtlSeconds { get; set; } = 300;
}
