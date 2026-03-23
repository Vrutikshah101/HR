using LeaveManagement.API.Contracts.Leaves;
using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Leaves;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/leaves")]
[Authorize]
public class LeavesController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;
    private readonly ILeaveWorkflowService _leaveWorkflowService;
    private readonly ICurrentUserService _currentUserService;

    public LeavesController(
        ILeaveRequestService leaveRequestService,
        ILeaveWorkflowService leaveWorkflowService,
        ICurrentUserService currentUserService)
    {
        _leaveRequestService = leaveRequestService;
        _leaveWorkflowService = leaveWorkflowService;
        _currentUserService = currentUserService;
    }

    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<LeaveTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTypes(CancellationToken cancellationToken)
    {
        var types = await _leaveRequestService.GetLeaveTypesAsync(cancellationToken);
        return Ok(types.Select(x => new LeaveTypeResponse(x.Code, x.Name)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(LeaveResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Apply([FromBody] ApplyLeaveRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var leave = await _leaveRequestService.ApplyAsync(
                _currentUserService.UserId,
                new ApplyLeaveCommand(request.LeaveTypeCode, request.StartDate, request.EndDate, request.Days, request.Reason),
                cancellationToken);

            var response = ToResponse(leave);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<LeaveResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy(CancellationToken cancellationToken)
    {
        var leaves = await _leaveRequestService.GetMyAsync(_currentUserService.UserId, cancellationToken);
        return Ok(leaves.Select(ToResponse));
    }

    [HttpGet("team-pending")]
    [ProducesResponseType(typeof(IEnumerable<LeaveResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeamPending(CancellationToken cancellationToken)
    {
        var leaves = await _leaveRequestService.GetTeamPendingAsync(_currentUserService.UserId, cancellationToken);
        return Ok(leaves.Select(ToResponse));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LeaveResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var leave = await _leaveRequestService.GetByIdAsync(_currentUserService.UserId, _currentUserService.Roles, id, cancellationToken);

        if (leave is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(leave));
    }

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(typeof(LeaveResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] LeaveActionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var leave = await _leaveWorkflowService.ApproveAsync(_currentUserService.UserId, id, request.Comment, cancellationToken);
            return Ok(ToResponse(leave));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(typeof(LeaveResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] LeaveActionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var leave = await _leaveWorkflowService.RejectAsync(_currentUserService.UserId, id, request.Comment, cancellationToken);
            return Ok(ToResponse(leave));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(LeaveResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var leave = await _leaveRequestService.CancelAsync(_currentUserService.UserId, id, cancellationToken);
            return Ok(ToResponse(leave));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static LeaveResponse ToResponse(LeaveManagement.Domain.Entities.LeaveRequest leave)
    {
        return new LeaveResponse(
            leave.Id,
            leave.EmployeeId,
            leave.LeaveTypeCode,
            leave.StartDate,
            leave.EndDate,
            leave.Days,
            leave.Status,
            leave.Reason);
    }
}
