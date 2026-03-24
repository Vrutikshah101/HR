namespace LeaveManagement.Application.Users;

public sealed record UpdateUserProfileCommand(
    string FullName,
    string Department,
    string Designation);
