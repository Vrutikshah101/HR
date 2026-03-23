namespace LeaveManagement.Application.Abstractions;

public interface ICurrentUserService
{
    Guid UserId { get; }
    IReadOnlyCollection<string> Roles { get; }
}
