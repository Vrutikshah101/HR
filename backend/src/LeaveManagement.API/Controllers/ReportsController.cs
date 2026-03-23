using LeaveManagement.API.Contracts.Reports;
using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Hr,Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("leave-balance")]
    public async Task<IActionResult> LeaveBalance(
        [FromQuery] string? department,
        [FromQuery] string? leaveTypeCode,
        [FromQuery] string? format,
        CancellationToken cancellationToken)
    {
        var rows = await _reportService.GetLeaveBalanceAsync(new ReportFilters(department, leaveTypeCode, null, null, null, format), cancellationToken);
        return Render(rows, format, "leave-balance");
    }

    [HttpGet("department-summary")]
    public async Task<IActionResult> DepartmentSummary(
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] string? format,
        CancellationToken cancellationToken)
    {
        var rows = await _reportService.GetDepartmentSummaryAsync(new ReportFilters(null, null, dateFrom, dateTo, null, format), cancellationToken);
        return Render(rows, format, "department-summary");
    }

    [HttpGet("monthly-utilization")]
    public async Task<IActionResult> MonthlyUtilization(
        [FromQuery] int? year,
        [FromQuery] string? format,
        CancellationToken cancellationToken)
    {
        var rows = await _reportService.GetMonthlyUtilizationAsync(new ReportFilters(null, null, null, null, year, format), cancellationToken);
        return Render(rows, format, "monthly-utilization");
    }

    [HttpGet("approval-summary")]
    public async Task<IActionResult> ApprovalSummary(
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] string? format,
        CancellationToken cancellationToken)
    {
        var rows = await _reportService.GetApprovalSummaryAsync(new ReportFilters(null, null, dateFrom, dateTo, null, format), cancellationToken);
        return Render(rows, format, "approval-summary");
    }

    private IActionResult Render(IReadOnlyCollection<ReportRowDto> rows, string? format, string filePrefix)
    {
        if (string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
        {
            var bytes = _reportService.ToCsv(rows);
            return File(bytes, "text/csv", $"{filePrefix}.csv");
        }

        return Ok(rows.Select(x => new ReportRowResponse(x.Fields)));
    }
}
