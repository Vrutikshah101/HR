using LeaveManagement.API.Contracts.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    [HttpGet("employee")]
    [Authorize(Roles = "User,Hr,Admin")]
    public IActionResult Employee()
    {
        var cards = new[]
        {
            new DashboardCardResponse("availableLeave", "Available Leave", 10),
            new DashboardCardResponse("pendingRequests", "Pending Requests", 2)
        };

        return Ok(cards);
    }

    [HttpGet("manager")]
    [Authorize(Roles = "User,Hr,Admin")]
    public IActionResult Manager() => Ok(Array.Empty<DashboardCardResponse>());

    [HttpGet("hr")]
    [Authorize(Roles = "Hr,Admin")]
    public IActionResult Hr() => Ok(Array.Empty<DashboardCardResponse>());

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult Admin() => Ok(Array.Empty<DashboardCardResponse>());
}
