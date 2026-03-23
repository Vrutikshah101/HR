namespace LeaveManagement.API.Contracts.Auth;

public sealed record LoginResponse(string AccessToken, string RefreshToken);
