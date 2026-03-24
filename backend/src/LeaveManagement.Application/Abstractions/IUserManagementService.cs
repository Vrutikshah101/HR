using LeaveManagement.Application.Users;

namespace LeaveManagement.Application.Abstractions;

public interface IUserManagementService
{
    Task<CreateUserResult> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<UserSummary>> GetUsersAsync(CancellationToken cancellationToken);
    Task<UserProfile?> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserProfile> UpdateMyProfileAsync(Guid userId, UpdateUserProfileCommand command, CancellationToken cancellationToken);
}
