using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Users;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Services;

public class DevelopmentDataSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public DevelopmentDataSeeder(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        await CreateUserAsync(new CreateUserCommand(
            "admin@leave.local",
            "Admin@123",
            "ADM001",
            "System Admin",
            "Administration",
            "Administrator",
            new[] { "Admin" }), cancellationToken);

        await CreateUserAsync(new CreateUserCommand(
            "hr@leave.local",
            "Hr@12345",
            "HR001",
            "HR Manager",
            "HR",
            "HR Manager",
            new[] { "Hr" }), cancellationToken);

        var manager = await CreateUserAsync(new CreateUserCommand(
            "manager@leave.local",
            "User@12345",
            "EMP100",
            "Team Manager",
            "Engineering",
            "Engineering Manager",
            new[] { "User" }), cancellationToken);

        var employee = await CreateUserAsync(new CreateUserCommand(
            "employee@leave.local",
            "User@12345",
            "EMP101",
            "Team Employee",
            "Engineering",
            "Software Engineer",
            new[] { "User" }), cancellationToken);

        await _dbContext.ReportingHierarchies.AddAsync(new ReportingHierarchy
        {
            Id = Guid.NewGuid(),
            EmployeeId = employee.EmployeeId,
            Level1ApproverEmployeeId = manager.EmployeeId,
            Level2ApproverEmployeeId = null
        }, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<CreateUserResult> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email.Trim().ToLowerInvariant(),
            PasswordHash = _passwordHasher.Hash(command.Password),
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var roles = command.Roles
            .Select(x => Enum.Parse<UserRole>(x, ignoreCase: true))
            .Distinct()
            .Select(role => new UserRoleAssignment
            {
                UserId = user.Id,
                RoleCode = role
            });

        user.RoleAssignments = roles.ToList();

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            EmployeeCode = command.EmployeeCode,
            FullName = command.FullName,
            Department = command.Department,
            Designation = command.Designation
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateUserResult(user.Id, employee.Id);
    }
}
