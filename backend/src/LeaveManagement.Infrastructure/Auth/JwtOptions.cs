namespace LeaveManagement.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "LeaveManagement";
    public string Audience { get; set; } = "LeaveManagement.Client";
    public string SigningKey { get; set; } = "change-this-in-production-very-long-key";
    public int AccessTokenMinutes { get; set; } = 60;
}
