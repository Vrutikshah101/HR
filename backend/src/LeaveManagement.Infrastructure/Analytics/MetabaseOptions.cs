namespace LeaveManagement.Infrastructure.Analytics;

public sealed class MetabaseOptions
{
    public const string SectionName = "Metabase";

    public bool Enabled { get; set; }
    public string SiteUrl { get; set; } = string.Empty;
    public string EmbedSecret { get; set; } = string.Empty;
    public int TokenTtlMinutes { get; set; } = 10;
    public int DefaultDashboardId { get; set; } = 1;
    public int UserDashboardId { get; set; } = 1;
    public int HrDashboardId { get; set; } = 1;
    public int AdminDashboardId { get; set; } = 1;
}
