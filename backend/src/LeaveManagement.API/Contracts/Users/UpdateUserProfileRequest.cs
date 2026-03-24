namespace LeaveManagement.API.Contracts.Users;

public sealed record UpdateUserProfileRequest(
    string FullName,
    string Department,
    string Designation);
