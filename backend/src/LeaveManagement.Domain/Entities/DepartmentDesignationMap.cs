namespace LeaveManagement.Domain.Entities;

public class DepartmentDesignationMap
{
    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid DesignationId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
