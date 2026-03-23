namespace LeaveManagement.Infrastructure.Services;

public sealed class HolidayOption
{
    public DateOnly Date { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class LeaveTypeOption
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal DefaultOpeningBalance { get; set; } = 12;
}

public sealed class LeaveOptions
{
    public const string SectionName = "Leave";

    public List<LeaveTypeOption> Types { get; set; } =
    [
        new() { Code = "CL", Name = "Casual Leave", DefaultOpeningBalance = 12 },
        new() { Code = "SL", Name = "Sick Leave", DefaultOpeningBalance = 10 },
        new() { Code = "EL", Name = "Earned Leave", DefaultOpeningBalance = 15 }
    ];

    public List<HolidayOption> Holidays { get; set; } = [];
}
