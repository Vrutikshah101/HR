using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Users;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public UserManagementService(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
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
            Designation = command.Designation.Trim()
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

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
                x.user.RoleAssignments.Select(r => r.RoleCode.ToString()).OrderBy(r => r).ToArray(),
                x.user.IsActive))
            .ToArray();
    }
}
