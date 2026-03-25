namespace LeaveManagement.API.Contracts.Reports;

public sealed record AnalyticsEmbedResponse(string EmbedUrl, int DashboardId, long ExpiresAtUtcUnix);
