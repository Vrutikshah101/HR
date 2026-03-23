using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Domain.Entities;

public class LeaveRequest
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string LeaveTypeCode { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal Days { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Draft;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
