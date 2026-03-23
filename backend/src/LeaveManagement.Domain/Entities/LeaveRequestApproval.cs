namespace LeaveManagement.Domain.Entities;

public class LeaveRequestApproval
{
    public Guid Id { get; set; }
    public Guid LeaveRequestId { get; set; }
    public int ApprovalLevel { get; set; }
    public Guid ApproverEmployeeId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTimeOffset ActionedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
