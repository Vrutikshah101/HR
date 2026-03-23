namespace LeaveManagement.Application.Hierarchy;

public sealed record HierarchyDto(Guid EmployeeId, Guid? Level1ApproverEmployeeId, Guid? Level2ApproverEmployeeId);
