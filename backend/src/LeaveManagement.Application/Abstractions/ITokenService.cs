namespace LeaveManagement.Application.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, IEnumerable<string> roles);
}
