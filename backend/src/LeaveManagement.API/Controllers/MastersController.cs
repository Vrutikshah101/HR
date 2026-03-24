using LeaveManagement.API.Contracts.Masters;
using LeaveManagement.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/masters")]
[Authorize(Roles = "Admin,Hr")]
public class MastersController : ControllerBase
{
    private readonly IMasterDataService _masterDataService;

    public MastersController(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
    }

    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
    {
        var rows = await _masterDataService.GetDepartmentsAsync(cancellationToken);
        return Ok(rows);
    }

    [HttpPost("departments")]
    public async Task<IActionResult> CreateDepartment([FromBody] DepartmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var row = await _masterDataService.CreateDepartmentAsync(request.Code, request.Name, cancellationToken);
            return Ok(row);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("designations")]
    public async Task<IActionResult> GetDesignations(CancellationToken cancellationToken)
    {
        var rows = await _masterDataService.GetDesignationsAsync(cancellationToken);
        return Ok(rows);
    }

    [HttpGet("departments/{departmentId:guid}/designations")]
    public async Task<IActionResult> GetDesignationsByDepartment(Guid departmentId, CancellationToken cancellationToken)
    {
        var rows = await _masterDataService.GetDesignationsByDepartmentAsync(departmentId, cancellationToken);
        return Ok(rows);
    }

    [HttpPost("designations")]
    public async Task<IActionResult> CreateDesignation([FromBody] DesignationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var row = await _masterDataService.CreateDesignationAsync(request.Code, request.Name, cancellationToken);
            return Ok(row);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("department-designation-maps")]
    public async Task<IActionResult> GetDepartmentDesignationMaps(CancellationToken cancellationToken)
    {
        var rows = await _masterDataService.GetDepartmentDesignationMapsAsync(cancellationToken);
        return Ok(rows);
    }

    [HttpPost("department-designation-maps")]
    public async Task<IActionResult> CreateDepartmentDesignationMap([FromBody] DepartmentDesignationMapRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var row = await _masterDataService.CreateDepartmentDesignationMapAsync(request.DepartmentId, request.DesignationId, cancellationToken);
            return Ok(row);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("leave-types")]
    public async Task<IActionResult> GetLeaveTypes(CancellationToken cancellationToken)
    {
        var rows = await _masterDataService.GetLeaveTypesAsync(cancellationToken);
        return Ok(rows);
    }

    [HttpPost("leave-types")]
    public async Task<IActionResult> CreateLeaveType([FromBody] LeaveTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var row = await _masterDataService.CreateLeaveTypeAsync(
                request.Code,
                request.Name,
                request.RequiresAttachment,
                request.IsPaid,
                request.IsHalfDayAllowed,
                request.IsBackdatedAllowed,
                request.MaxDaysPerRequest,
                cancellationToken);

            return Ok(row);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("holidays")]
    public async Task<IActionResult> GetHolidays(CancellationToken cancellationToken)
    {
        var rows = await _masterDataService.GetHolidaysAsync(cancellationToken);
        return Ok(rows);
    }

    [HttpPost("holidays")]
    public async Task<IActionResult> CreateHoliday([FromBody] HolidayRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var row = await _masterDataService.CreateHolidayAsync(request.Name, request.Date, request.Location, request.IsOptional, cancellationToken);
            return Ok(row);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
