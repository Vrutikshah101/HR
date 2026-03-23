namespace LeaveManagement.Domain.Entities;

public class LeaveBalance
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string LeaveTypeCode { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; }
    public decimal Used { get; set; }
    public decimal Adjustments { get; set; }

    public decimal Available => OpeningBalance + Adjustments - Used;
}
