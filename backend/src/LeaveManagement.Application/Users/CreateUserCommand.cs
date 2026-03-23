namespace LeaveManagement.Application.Users;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string EmployeeCode,
    string FullName,
    string Department,
    string Designation,
    IReadOnlyCollection<string> Roles);
