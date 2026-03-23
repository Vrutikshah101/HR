namespace LeaveManagement.API.Contracts.Hierarchy;

public sealed record HierarchyResponse(Guid EmployeeId, Guid? Level1ApproverEmployeeId, Guid? Level2ApproverEmployeeId);
