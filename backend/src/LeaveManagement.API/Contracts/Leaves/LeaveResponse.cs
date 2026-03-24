using LeaveManagement.Domain.Enums;

namespace LeaveManagement.API.Contracts.Leaves;

public sealed record LeaveResponse(
    Guid Id,
    Guid EmployeeId,
    string? EmployeeName,
    string? EmployeeEmail,
    string LeaveTypeCode,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Days,
    LeaveRequestStatus Status,
    string Reason
);
