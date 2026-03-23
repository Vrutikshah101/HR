namespace LeaveManagement.Application.Auth;

public sealed record LoginResult(Guid UserId, string AccessToken, string RefreshToken, IReadOnlyCollection<string> Roles);
