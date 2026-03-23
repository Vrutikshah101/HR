namespace LeaveManagement.Domain.Enums;

public enum LeaveRequestStatus
{
    Draft = 1,
    PendingLevel1 = 2,
    PendingLevel2 = 3,
    Approved = 4,
    Rejected = 5,
    Cancelled = 6
}
