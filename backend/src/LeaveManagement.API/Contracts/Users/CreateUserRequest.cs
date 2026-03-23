namespace LeaveManagement.API.Contracts.Users;

public sealed record CreateUserRequest(
    string Email,
    string Password,
    string EmployeeCode,
    string FullName,
    string Department,
    string Designation,
    IReadOnlyCollection<string> Roles);
