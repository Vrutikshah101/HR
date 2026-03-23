using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Auth;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResult?> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
            .Include(x => x.RoleAssignments)
            .SingleOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        if (!_passwordHasher.Verify(password, user.PasswordHash))
        {
            return null;
        }

        var roles = user.RoleAssignments
            .Select(x => x.RoleCode.ToString())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var accessToken = _tokenService.GenerateAccessToken(user.Id, roles);
        return new LoginResult(user.Id, accessToken, "not-implemented", roles);
    }
}
