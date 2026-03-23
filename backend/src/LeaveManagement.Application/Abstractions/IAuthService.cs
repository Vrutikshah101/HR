using LeaveManagement.Application.Auth;

namespace LeaveManagement.Application.Abstractions;

public interface IAuthService
{
    Task<LoginResult?> LoginAsync(string email, string password, CancellationToken cancellationToken);
}
