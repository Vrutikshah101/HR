namespace LeaveManagement.Application.Masters;

public sealed record LeaveTypeMasterDto(
    Guid Id,
    string Code,
    string Name,
    bool RequiresAttachment,
    bool IsPaid,
    bool IsHalfDayAllowed,
    bool IsBackdatedAllowed,
    decimal? MaxDaysPerRequest,
    bool IsActive);
