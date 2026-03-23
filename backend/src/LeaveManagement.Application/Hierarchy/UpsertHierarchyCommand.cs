namespace LeaveManagement.Application.Hierarchy;

public sealed record UpsertHierarchyCommand(Guid EmployeeId, Guid? Level1ApproverEmployeeId, Guid? Level2ApproverEmployeeId);
