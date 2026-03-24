namespace LeaveManagement.API.Contracts.Masters;

public sealed record LeaveTypeRequest(
    string Code,
    string Name,
    bool RequiresAttachment,
    bool IsPaid,
    bool IsHalfDayAllowed,
    bool IsBackdatedAllowed,
    decimal? MaxDaysPerRequest);
