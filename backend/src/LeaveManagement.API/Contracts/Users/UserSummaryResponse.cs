namespace LeaveManagement.API.Contracts.Users;

public sealed record UserSummaryResponse(
    Guid UserId,
    Guid EmployeeId,
    string Email,
    string EmployeeCode,
    string FullName,
    string Department,
    string Designation,
    IReadOnlyCollection<string> Roles,
    bool IsActive);
