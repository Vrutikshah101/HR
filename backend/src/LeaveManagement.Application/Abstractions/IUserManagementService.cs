using LeaveManagement.Application.Users;

namespace LeaveManagement.Application.Abstractions;

public interface IUserManagementService
{
    Task<CreateUserResult> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<UserSummary>> GetUsersAsync(CancellationToken cancellationToken);
}
