using LeaveManagement.API.Contracts.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Hr,Admin")]
public class ReportsController : ControllerBase
{
    [HttpGet("leave-balance")]
    public IActionResult LeaveBalance()
    {
        return Ok(new[] { new ReportRowResponse(new Dictionary<string, object?> { ["employeeCode"] = "E001", ["balance"] = 10m }) });
    }

    [HttpGet("department-summary")]
    public IActionResult DepartmentSummary() => Ok(Array.Empty<ReportRowResponse>());

    [HttpGet("monthly-utilization")]
    public IActionResult MonthlyUtilization() => Ok(Array.Empty<ReportRowResponse>());

    [HttpGet("approval-summary")]
    public IActionResult ApprovalSummary() => Ok(Array.Empty<ReportRowResponse>());
}
