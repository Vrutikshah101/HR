namespace LeaveManagement.Application.Users;

public sealed record UserSummary(
    Guid UserId,
    Guid EmployeeId,
    string Email,
    string EmployeeCode,
    string FullName,
    string Department,
    string Designation,
    IReadOnlyCollection<string> Roles,
    bool IsActive);
