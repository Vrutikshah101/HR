namespace LeaveManagement.Domain.Entities;

public class LeaveTypeMaster
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool RequiresAttachment { get; set; }
    public bool IsPaid { get; set; } = true;
    public bool IsHalfDayAllowed { get; set; } = true;
    public bool IsBackdatedAllowed { get; set; }
    public decimal? MaxDaysPerRequest { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
