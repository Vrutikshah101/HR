using LeaveManagement.Application.Reports;

namespace LeaveManagement.Application.Abstractions;

public interface IReportService
{
    Task<IReadOnlyCollection<ReportRowDto>> GetLeaveBalanceAsync(ReportFilters filters, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ReportRowDto>> GetDepartmentSummaryAsync(ReportFilters filters, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ReportRowDto>> GetMonthlyUtilizationAsync(ReportFilters filters, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ReportRowDto>> GetApprovalSummaryAsync(ReportFilters filters, CancellationToken cancellationToken);
    byte[] ToCsv(IReadOnlyCollection<ReportRowDto> rows);
}
