namespace LeaveManagement.Application.Balances;

public sealed record LeaveBalanceSummaryDto(string LeaveTypeCode, decimal OpeningBalance, decimal Used, decimal Adjustments, decimal Available);
