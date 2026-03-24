using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Users;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enums;
using LeaveManagement.Infrastructure.Persistence;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LeaveManagement.Infrastructure.Services;

public class DevelopmentDataSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly LeaveOptions _leaveOptions;

    public DevelopmentDataSeeder(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, IOptions<LeaveOptions> leaveOptions)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _leaveOptions = leaveOptions.Value;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await SeedMasterDataAsync(cancellationToken);

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
            "MALE",
            null,
            new DateOnly(2020, 1, 1),
            null,
            new[] { "Admin" }), cancellationToken);

        await CreateUserAsync(new CreateUserCommand(
            "hr@leave.local",
            "Hr@12345",
            "HR001",
            "HR Manager",
            "HR",
            "HR Manager",
            "FEMALE",
            null,
            new DateOnly(2021, 2, 1),
            null,
            new[] { "Hr" }), cancellationToken);

        var manager = await CreateUserAsync(new CreateUserCommand(
            "manager@leave.local",
            "User@12345",
            "EMP100",
            "Team Manager",
            "Engineering",
            "Engineering Manager",
            "MALE",
            null,
            new DateOnly(2022, 1, 10),
            null,
            new[] { "User" }), cancellationToken);

        var employee = await CreateUserAsync(new CreateUserCommand(
            "employee@leave.local",
            "User@12345",
            "EMP101",
            "Team Employee",
            "Engineering",
            "Software Engineer",
            "OTHER",
            null,
            new DateOnly(2023, 6, 12),
            null,
            new[] { "User" }), cancellationToken);

        await _dbContext.ReportingHierarchies.AddAsync(new ReportingHierarchy
        {
            Id = Guid.NewGuid(),
            EmployeeId = employee.EmployeeId,
            Level1ApproverEmployeeId = manager.EmployeeId,
            Level2ApproverEmployeeId = null
        }, cancellationToken);

        await SeedBalancesAsync(employee.EmployeeId, cancellationToken);
        await SeedBalancesAsync(manager.EmployeeId, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedMasterDataAsync(CancellationToken cancellationToken)
    {
        await EnsureSchemaCompatibilityAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;

        var departmentsToEnsure = new[]
        {
            ("ADMIN", "Administration"),
            ("HR", "Human Resources"),
            ("ENG", "Engineering"),
            ("HW", "Hardware")
        };

        var existingDepartmentCodes = await _dbContext.Departments
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        foreach (var (code, name) in departmentsToEnsure)
        {
            if (existingDepartmentCodes.Contains(code, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            await _dbContext.Departments.AddAsync(new Department
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name,
                IsActive = true,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            }, cancellationToken);
        }

        var designationsToEnsure = new[]
        {
            ("ADMIN", "Administrator"),
            ("HRMGR", "HR Manager"),
            ("ENGMGR", "Engineering Manager"),
            ("SWE", "Software Engineer"),
            ("MGR_ENG", "Manager - Engineering"),
            ("MGR_HW", "Manager - HW"),
            ("ASST_MGR", "Assistant Manager"),
            ("EXEC", "Executive"),
            ("GM", "General Manager")
        };

        var existingDesignationCodes = await _dbContext.Designations
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        foreach (var (code, name) in designationsToEnsure)
        {
            if (existingDesignationCodes.Contains(code, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            await _dbContext.Designations.AddAsync(new Designation
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name,
                IsActive = true,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            }, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var departmentsByCode = await _dbContext.Departments
            .ToDictionaryAsync(x => x.Code, x => x.Id, cancellationToken);
        var designationsByCode = await _dbContext.Designations
            .ToDictionaryAsync(x => x.Code, x => x.Id, cancellationToken);

        var existingMappings = await _dbContext.DepartmentDesignationMaps
            .Select(x => new { x.DepartmentId, x.DesignationId })
            .ToListAsync(cancellationToken);

        var mappingKeys = new HashSet<string>(
            existingMappings.Select(x => $"{x.DepartmentId:N}|{x.DesignationId:N}"),
            StringComparer.OrdinalIgnoreCase);

        var explicitMappings = new[]
        {
            ("ADMIN", "ADMIN"),
            ("HR", "HRMGR"),
            ("ENG", "ENGMGR"),
            ("ENG", "SWE"),
            ("ENG", "MGR_ENG"),
            ("HW", "MGR_HW")
        };

        foreach (var (departmentCode, designationCode) in explicitMappings)
        {
            if (!departmentsByCode.TryGetValue(departmentCode, out var departmentId)
                || !designationsByCode.TryGetValue(designationCode, out var designationId))
            {
                continue;
            }

            var key = $"{departmentId:N}|{designationId:N}";
            if (mappingKeys.Contains(key))
            {
                continue;
            }

            await _dbContext.DepartmentDesignationMaps.AddAsync(new DepartmentDesignationMap
            {
                Id = Guid.NewGuid(),
                DepartmentId = departmentId,
                DesignationId = designationId,
                CreatedAtUtc = now
            }, cancellationToken);
            mappingKeys.Add(key);
        }

        var commonDesignationCodes = new[] { "ASST_MGR", "EXEC", "GM" };
        foreach (var departmentId in departmentsByCode.Values)
        {
            foreach (var commonDesignationCode in commonDesignationCodes)
            {
                if (!designationsByCode.TryGetValue(commonDesignationCode, out var designationId))
                {
                    continue;
                }

                var key = $"{departmentId:N}|{designationId:N}";
                if (mappingKeys.Contains(key))
                {
                    continue;
                }

                await _dbContext.DepartmentDesignationMaps.AddAsync(new DepartmentDesignationMap
                {
                    Id = Guid.NewGuid(),
                    DepartmentId = departmentId,
                    DesignationId = designationId,
                    CreatedAtUtc = now
                }, cancellationToken);
                mappingKeys.Add(key);
            }
        }

        if (!await _dbContext.LeaveTypes.AnyAsync(cancellationToken))
        {
            var existingCodes = await _dbContext.LeaveTypes
                .Select(x => x.Code)
                .ToListAsync(cancellationToken);

            var uniqueTypes = _leaveOptions.Types
                .Where(x => !string.IsNullOrWhiteSpace(x.Code) && !string.IsNullOrWhiteSpace(x.Name))
                .Select(x => new
                {
                    Code = x.Code.Trim().ToUpperInvariant(),
                    Name = x.Name.Trim()
                })
                .DistinctBy(x => x.Code)
                .Where(x => !existingCodes.Contains(x.Code, StringComparer.OrdinalIgnoreCase))
                .ToArray();

            foreach (var type in uniqueTypes)
            {
                await _dbContext.LeaveTypes.AddAsync(new LeaveTypeMaster
                {
                    Id = Guid.NewGuid(),
                    Code = type.Code,
                    Name = type.Name,
                    IsPaid = true,
                    IsHalfDayAllowed = true,
                    RequiresAttachment = false,
                    IsBackdatedAllowed = false,
                    MaxDaysPerRequest = null,
                    IsActive = true,
                    CreatedAtUtc = DateTimeOffset.UtcNow,
                    UpdatedAtUtc = DateTimeOffset.UtcNow
                }, cancellationToken);
            }
        }

        if (!await _dbContext.Holidays.AnyAsync(cancellationToken))
        {
            foreach (var holiday in _leaveOptions.Holidays)
            {
                await _dbContext.Holidays.AddAsync(new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = holiday.Name,
                    Date = holiday.Date,
                    Location = null,
                    IsOptional = false,
                    CreatedAtUtc = DateTimeOffset.UtcNow
                }, cancellationToken);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureSchemaCompatibilityAsync(CancellationToken cancellationToken)
    {
        // Safeguard for environments where migration history exists but some schema objects are missing.
        await _dbContext.Database.ExecuteSqlRawAsync("""
CREATE TABLE IF NOT EXISTS `department_designation_maps` (
  `id` char(36) NOT NULL,
  `department_id` char(36) NOT NULL,
  `designation_id` char(36) NOT NULL,
  `created_at_utc` datetime(6) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `IX_department_designation_maps_department_id_designation_id` (`department_id`,`designation_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
""", cancellationToken);

        // Keep employee columns aligned with current entity model for older local databases.
        await EnsureEmployeeColumnAsync("date_of_birth", "ALTER TABLE `employees` ADD COLUMN `date_of_birth` date NULL;", cancellationToken);
        await EnsureEmployeeColumnAsync("gender", "ALTER TABLE `employees` ADD COLUMN `gender` varchar(20) NULL;", cancellationToken);
        await EnsureEmployeeColumnAsync("join_date", "ALTER TABLE `employees` ADD COLUMN `join_date` date NULL;", cancellationToken);
        await EnsureEmployeeColumnAsync("date_of_relieving", "ALTER TABLE `employees` ADD COLUMN `date_of_relieving` date NULL;", cancellationToken);
    }

    private async Task EnsureEmployeeColumnAsync(string columnName, string alterSql, CancellationToken cancellationToken)
    {
        if (await ColumnExistsAsync("employees", columnName, cancellationToken))
        {
            return;
        }

        await _dbContext.Database.ExecuteSqlRawAsync(alterSql, cancellationToken);
    }

    private async Task<bool> ColumnExistsAsync(string tableName, string columnName, CancellationToken cancellationToken)
    {
        var connection = _dbContext.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
SELECT COUNT(*)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = @table
  AND COLUMN_NAME = @column;
""";

            var tableParameter = command.CreateParameter();
            tableParameter.ParameterName = "@table";
            tableParameter.Value = tableName;
            command.Parameters.Add(tableParameter);

            var columnParameter = command.CreateParameter();
            columnParameter.ParameterName = "@column";
            columnParameter.Value = columnName;
            command.Parameters.Add(columnParameter);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result) > 0;
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
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
            Designation = command.Designation,
            Gender = command.Gender,
            DateOfBirth = command.DateOfBirth,
            JoinDate = command.JoinDate,
            DateOfRelieving = command.DateOfRelieving
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateUserResult(user.Id, employee.Id);
    }

    private async Task SeedBalancesAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        foreach (var leaveType in _leaveOptions.Types)
        {
            await _dbContext.LeaveBalances.AddAsync(new LeaveBalance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                LeaveTypeCode = leaveType.Code.Trim().ToUpperInvariant(),
                OpeningBalance = leaveType.DefaultOpeningBalance,
                Used = 0,
                Adjustments = 0
            }, cancellationToken);
        }
    }
}
