namespace LeaveManagement.API.Contracts.Hierarchy;

public sealed record UpsertHierarchyRequest(Guid EmployeeId, Guid? Level1ApproverEmployeeId, Guid? Level2ApproverEmployeeId);
