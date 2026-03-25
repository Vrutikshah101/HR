namespace LeaveManagement.Application.Reports;

public sealed record AnalyticsEmbedResult(string EmbedUrl, int DashboardId, long ExpiresAtUtcUnix);
