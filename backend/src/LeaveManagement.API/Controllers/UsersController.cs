using LeaveManagement.API.Contracts.Users;
using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(IUserManagementService userManagementService, ICurrentUserService currentUserService)
    {
        _userManagementService = userManagementService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Hr")]
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
                    request.Gender,
                    request.DateOfBirth,
                    request.JoinDate,
                    request.DateOfRelieving,
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
    [Authorize(Roles = "Admin,Hr")]
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
            x.Gender,
            x.DateOfBirth,
            x.JoinDate,
            x.DateOfRelieving,
            x.Roles,
            x.IsActive)));
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var profile = await _userManagementService.GetMyProfileAsync(_currentUserService.UserId, cancellationToken);
        if (profile is null)
        {
            return NotFound();
        }

        return Ok(new UserProfileResponse(
            profile.Email,
            profile.EmployeeCode,
            profile.FullName,
            profile.Department,
            profile.Designation,
            profile.Gender,
            profile.DateOfBirth,
            profile.JoinDate,
            profile.DateOfRelieving,
            profile.Roles,
            profile.IsActive));
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await _userManagementService.UpdateMyProfileAsync(
                _currentUserService.UserId,
                new UpdateUserProfileCommand(request.FullName, request.Department, request.Designation),
                cancellationToken);

            return Ok(new UserProfileResponse(
                profile.Email,
                profile.EmployeeCode,
                profile.FullName,
                profile.Department,
                profile.Designation,
                profile.Gender,
                profile.DateOfBirth,
                profile.JoinDate,
                profile.DateOfRelieving,
                profile.Roles,
                profile.IsActive));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            var root = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(new { message = $"Registration failed due to database update error: {root}" });
        }
    }
}
