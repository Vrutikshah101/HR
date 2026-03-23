namespace LeaveManagement.API.Contracts.Leaves;

public sealed record ApplyLeaveRequest(
    string LeaveTypeCode,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Days,
    string Reason
);
