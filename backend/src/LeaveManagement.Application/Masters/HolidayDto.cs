namespace LeaveManagement.Application.Masters;

public sealed record HolidayDto(Guid Id, string Name, DateOnly Date, string? Location, bool IsOptional);
