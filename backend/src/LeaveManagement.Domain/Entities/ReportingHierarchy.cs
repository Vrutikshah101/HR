namespace LeaveManagement.Domain.Entities;

public class ReportingHierarchy
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid? Level1ApproverEmployeeId { get; set; }
    public Guid? Level2ApproverEmployeeId { get; set; }
}
