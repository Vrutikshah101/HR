namespace LeaveManagement.Application.Abstractions;

public interface IAnalyticsEmbedService
{
    Task<LeaveManagement.Application.Reports.AnalyticsEmbedResult> BuildDashboardEmbedUrlAsync(
        IReadOnlyCollection<string> roles,
        int? dashboardId,
        CancellationToken cancellationToken);
}
