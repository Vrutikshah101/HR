namespace LeaveManagement.API.Contracts.Masters;

public sealed record HolidayRequest(string Name, DateOnly Date, string? Location, bool IsOptional);
