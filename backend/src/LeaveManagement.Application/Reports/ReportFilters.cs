namespace LeaveManagement.Application.Reports;

public sealed record ReportFilters(
    string? Department,
    string? LeaveTypeCode,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    int? Year,
    string? Format);
