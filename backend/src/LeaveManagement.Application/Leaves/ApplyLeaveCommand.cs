namespace LeaveManagement.Application.Leaves;

public sealed record ApplyLeaveCommand(
    string LeaveTypeCode,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Days,
    string Reason);
