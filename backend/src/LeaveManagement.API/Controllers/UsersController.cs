using LeaveManagement.API.Contracts.Users;
using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin,Hr")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UsersController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userManagementService.CreateUserAsync(
                new CreateUserCommand(
                    request.Email,
                    request.Password,
                    request.EmployeeCode,
                    request.FullName,
                    request.Department,
                    request.Designation,
                    request.Roles),
                cancellationToken);

            return CreatedAtAction(nameof(GetAll), new { }, new CreateUserResponse(result.UserId, result.EmployeeId));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userManagementService.GetUsersAsync(cancellationToken);
        return Ok(users.Select(x => new UserSummaryResponse(
            x.UserId,
            x.EmployeeId,
            x.Email,
            x.EmployeeCode,
            x.FullName,
            x.Department,
            x.Designation,
            x.Roles,
            x.IsActive)));
    }
}
