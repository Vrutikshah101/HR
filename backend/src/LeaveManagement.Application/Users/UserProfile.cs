namespace LeaveManagement.Application.Users;

public sealed record UserProfile(
    Guid UserId,
    Guid EmployeeId,
    string Email,
    string EmployeeCode,
    string FullName,
    string Department,
    string Designation,
    string? Gender,
    DateOnly? DateOfBirth,
    DateOnly? JoinDate,
    DateOnly? DateOfRelieving,
    IReadOnlyCollection<string> Roles,
    bool IsActive);
