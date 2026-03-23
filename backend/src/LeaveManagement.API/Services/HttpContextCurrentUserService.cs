using System.Security.Claims;
using LeaveManagement.Application.Abstractions;

namespace LeaveManagement.API.Services;

public class HttpContextCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId)
                ? userId
                : Guid.Empty;
        }
    }

    public IReadOnlyCollection<string> Roles => _httpContextAccessor.HttpContext?.User
        .FindAll(ClaimTypes.Role)
        .Select(x => x.Value)
        .ToArray() ?? Array.Empty<string>();
}
