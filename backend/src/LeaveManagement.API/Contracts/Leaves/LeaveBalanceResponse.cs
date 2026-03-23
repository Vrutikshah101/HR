namespace LeaveManagement.API.Contracts.Leaves;

public sealed record LeaveBalanceResponse(string LeaveTypeCode, decimal OpeningBalance, decimal Used, decimal Adjustments, decimal Available);
