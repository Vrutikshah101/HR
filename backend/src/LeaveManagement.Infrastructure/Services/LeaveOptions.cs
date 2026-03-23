namespace LeaveManagement.Infrastructure.Services;

public sealed class LeaveTypeOption
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class LeaveOptions
{
    public const string SectionName = "Leave";

    public List<LeaveTypeOption> Types { get; set; } =
    [
        new() { Code = "CL", Name = "Casual Leave" },
        new() { Code = "SL", Name = "Sick Leave" },
        new() { Code = "EL", Name = "Earned Leave" }
    ];
}
