namespace LeaveManagement.Application.Masters;

public sealed record DepartmentDesignationMapDto(
    Guid Id,
    Guid DepartmentId,
    string DepartmentName,
    Guid DesignationId,
    string DesignationName);
