using LeaveManagement.API.Contracts.Hierarchy;
using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Hierarchy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/hierarchy")]
[Authorize(Roles = "Admin,Hr")]
public class HierarchyController : ControllerBase
{
    private readonly IHierarchyService _hierarchyService;

    public HierarchyController(IHierarchyService hierarchyService)
    {
        _hierarchyService = hierarchyService;
    }

    [HttpPut]
    [ProducesResponseType(typeof(HierarchyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upsert([FromBody] UpsertHierarchyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var hierarchy = await _hierarchyService.UpsertAsync(
                new UpsertHierarchyCommand(request.EmployeeId, request.Level1ApproverEmployeeId, request.Level2ApproverEmployeeId),
                cancellationToken);

            return Ok(new HierarchyResponse(hierarchy.EmployeeId, hierarchy.Level1ApproverEmployeeId, hierarchy.Level2ApproverEmployeeId));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{employeeId:guid}")]
    [ProducesResponseType(typeof(HierarchyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmployeeId(Guid employeeId, CancellationToken cancellationToken)
    {
        var hierarchy = await _hierarchyService.GetByEmployeeIdAsync(employeeId, cancellationToken);

        if (hierarchy is null)
        {
            return NotFound();
        }

        return Ok(new HierarchyResponse(hierarchy.EmployeeId, hierarchy.Level1ApproverEmployeeId, hierarchy.Level2ApproverEmployeeId));
    }
}
