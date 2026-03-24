namespace LeaveManagement.Application.Leaves;

public sealed record EmployeeIdentityDto(
    Guid EmployeeId,
    string FullName,
    string Email);
