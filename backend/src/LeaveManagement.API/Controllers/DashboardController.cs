using LeaveManagement.API.Contracts.Dashboard;
using LeaveManagement.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ICurrentUserService _currentUserService;

    public DashboardController(IDashboardService dashboardService, ICurrentUserService currentUserService)
    {
        _dashboardService = dashboardService;
        _currentUserService = currentUserService;
    }

    [HttpGet("employee")]
    [Authorize(Roles = "User,Hr,Admin")]
    public async Task<IActionResult> Employee(CancellationToken cancellationToken)
    {
        var cards = await _dashboardService.GetEmployeeCardsAsync(_currentUserService.UserId, cancellationToken);
        return Ok(cards.Select(ToResponse));
    }

    [HttpGet("manager")]
    [Authorize(Roles = "User,Hr,Admin")]
    public async Task<IActionResult> Manager(CancellationToken cancellationToken)
    {
        var cards = await _dashboardService.GetManagerCardsAsync(_currentUserService.UserId, cancellationToken);
        return Ok(cards.Select(ToResponse));
    }

    [HttpGet("hr")]
    [Authorize(Roles = "Hr,Admin")]
    public async Task<IActionResult> Hr(CancellationToken cancellationToken)
    {
        var cards = await _dashboardService.GetHrCardsAsync(cancellationToken);
        return Ok(cards.Select(ToResponse));
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Admin(CancellationToken cancellationToken)
    {
        var cards = await _dashboardService.GetAdminCardsAsync(cancellationToken);
        return Ok(cards.Select(ToResponse));
    }

    private static DashboardCardResponse ToResponse(LeaveManagement.Application.Dashboard.DashboardCardDto dto)
        => new(dto.Key, dto.Label, dto.Value);
}
