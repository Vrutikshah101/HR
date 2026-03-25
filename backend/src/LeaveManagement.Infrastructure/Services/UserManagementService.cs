using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Users;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Infrastructure.Caching;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAppCache _appCache;

    public UserManagementService(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, IAppCache appCache)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _appCache = appCache;
    }

    public async Task<CreateUserResult> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        var employeeCode = command.EmployeeCode.Trim().ToUpperInvariant();

        if (await _dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        if (await _dbContext.Employees.AnyAsync(x => x.EmployeeCode == employeeCode, cancellationToken))
        {
            throw new InvalidOperationException("Employee code already exists.");
        }

        if (command.Roles.Count == 0)
        {
            throw new InvalidOperationException("At least one role is required.");
        }

        if (!IsStrongPassword(command.Password))
        {
            throw new InvalidOperationException("Password must be at least 8 characters and include uppercase, lowercase, digit, and special character.");
        }

        if (!string.IsNullOrWhiteSpace(command.Gender))
        {
            var normalizedGender = command.Gender.Trim().ToUpperInvariant();
            if (normalizedGender is not ("MALE" or "FEMALE" or "OTHER"))
            {
                throw new InvalidOperationException("Gender must be MALE, FEMALE, or OTHER.");
            }
        }

        var parsedRoles = command.Roles
            .Select(role => Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsed) ? parsed : (UserRole?)null)
            .ToArray();

        if (parsedRoles.Any(x => x is null))
        {
            throw new InvalidOperationException("One or more roles are invalid.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(command.Password),
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        user.RoleAssignments = parsedRoles
            .Select(x => new UserRoleAssignment { UserId = user.Id, RoleCode = x!.Value })
            .DistinctBy(x => x.RoleCode)
            .ToList();

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            EmployeeCode = employeeCode,
            FullName = command.FullName.Trim(),
            Department = command.Department.Trim(),
            Designation = command.Designation.Trim(),
            Gender = string.IsNullOrWhiteSpace(command.Gender) ? null : command.Gender.Trim().ToUpperInvariant(),
            DateOfBirth = command.DateOfBirth,
            JoinDate = command.JoinDate,
            DateOfRelieving = command.DateOfRelieving
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _appCache.RemoveAsync(CacheKeys.UserProfile(user.Id), cancellationToken);

        return new CreateUserResult(user.Id, employee.Id);
    }

    public async Task<IReadOnlyCollection<UserSummary>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users
            .Include(x => x.RoleAssignments)
            .Join(
                _dbContext.Employees,
                user => user.Id,
                employee => employee.UserId,
                (user, employee) => new { user, employee })
            .OrderBy(x => x.employee.EmployeeCode)
            .ToListAsync(cancellationToken);

        return users.Select(x => new UserSummary(
                x.user.Id,
                x.employee.Id,
                x.user.Email,
                x.employee.EmployeeCode,
                x.employee.FullName,
                x.employee.Department,
                x.employee.Designation,
                x.employee.Gender,
                x.employee.DateOfBirth,
                x.employee.JoinDate,
                x.employee.DateOfRelieving,
                x.user.RoleAssignments.Select(r => r.RoleCode.ToString()).OrderBy(r => r).ToArray(),
                x.user.IsActive))
            .ToArray();
    }

    public async Task<UserProfile?> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.UserProfile(userId);
        var cached = await _appCache.GetAsync<UserProfile>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var row = await _dbContext.Users
            .Include(x => x.RoleAssignments)
            .Join(
                _dbContext.Employees,
                user => user.Id,
                employee => employee.UserId,
                (user, employee) => new { user, employee })
            .SingleOrDefaultAsync(x => x.user.Id == userId, cancellationToken);

        if (row is null)
        {
            return null;
        }

        var profile = new UserProfile(
            row.user.Id,
            row.employee.Id,
            row.user.Email,
            row.employee.EmployeeCode,
            row.employee.FullName,
            row.employee.Department,
            row.employee.Designation,
            row.employee.Gender,
            row.employee.DateOfBirth,
            row.employee.JoinDate,
            row.employee.DateOfRelieving,
            row.user.RoleAssignments.Select(r => r.RoleCode.ToString()).OrderBy(r => r).ToArray(),
            row.user.IsActive);

        await _appCache.SetAsync(cacheKey, profile, CacheTtls.UserProfile, cancellationToken);
        return profile;
    }

    public async Task<UserProfile> UpdateMyProfileAsync(Guid userId, UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.FullName)
            || string.IsNullOrWhiteSpace(command.Department)
            || string.IsNullOrWhiteSpace(command.Designation))
        {
            throw new InvalidOperationException("Full name, department, and designation are required.");
        }

        var row = await _dbContext.Users
            .Include(x => x.RoleAssignments)
            .Join(
                _dbContext.Employees,
                user => user.Id,
                employee => employee.UserId,
                (user, employee) => new { user, employee })
            .SingleOrDefaultAsync(x => x.user.Id == userId, cancellationToken);

        if (row is null)
        {
            throw new InvalidOperationException("User profile not found.");
        }

        row.employee.FullName = command.FullName.Trim();
        row.employee.Department = command.Department.Trim();
        row.employee.Designation = command.Designation.Trim();

        await _dbContext.SaveChangesAsync(cancellationToken);

        var profile = new UserProfile(
            row.user.Id,
            row.employee.Id,
            row.user.Email,
            row.employee.EmployeeCode,
            row.employee.FullName,
            row.employee.Department,
            row.employee.Designation,
            row.employee.Gender,
            row.employee.DateOfBirth,
            row.employee.JoinDate,
            row.employee.DateOfRelieving,
            row.user.RoleAssignments.Select(r => r.RoleCode.ToString()).OrderBy(r => r).ToArray(),
            row.user.IsActive);

        await _appCache.RemoveAsync(CacheKeys.UserProfile(userId), cancellationToken);

        return profile;
    }

    private static bool IsStrongPassword(string password)
    {
        if (password.Length < 8)
        {
            return false;
        }

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
