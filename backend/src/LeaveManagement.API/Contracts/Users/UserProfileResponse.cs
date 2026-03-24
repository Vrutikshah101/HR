namespace LeaveManagement.API.Contracts.Users;

public sealed record UserProfileResponse(
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
