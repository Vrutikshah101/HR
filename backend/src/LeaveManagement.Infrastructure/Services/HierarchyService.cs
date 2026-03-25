using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Hierarchy;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Infrastructure.Caching;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Services;

public class HierarchyService : IHierarchyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IAppCache _appCache;

    public HierarchyService(ApplicationDbContext dbContext, IAppCache appCache)
    {
        _dbContext = dbContext;
        _appCache = appCache;
    }

    public async Task<HierarchyDto> UpsertAsync(UpsertHierarchyCommand command, CancellationToken cancellationToken)
    {
        var employeeExists = await _dbContext.Employees.AnyAsync(x => x.Id == command.EmployeeId, cancellationToken);
        if (!employeeExists)
        {
            throw new InvalidOperationException("Employee does not exist.");
        }

        if (command.Level1ApproverEmployeeId.HasValue)
        {
            var level1Exists = await _dbContext.Employees.AnyAsync(x => x.Id == command.Level1ApproverEmployeeId.Value, cancellationToken);
            if (!level1Exists)
            {
                throw new InvalidOperationException("Level1 approver employee does not exist.");
            }
        }

        if (command.Level2ApproverEmployeeId.HasValue)
        {
            var level2Exists = await _dbContext.Employees.AnyAsync(x => x.Id == command.Level2ApproverEmployeeId.Value, cancellationToken);
            if (!level2Exists)
            {
                throw new InvalidOperationException("Level2 approver employee does not exist.");
            }
        }

        if (command.EmployeeId == command.Level1ApproverEmployeeId || command.EmployeeId == command.Level2ApproverEmployeeId)
        {
            throw new InvalidOperationException("Employee cannot be their own approver.");
        }

        if (command.Level1ApproverEmployeeId.HasValue && command.Level1ApproverEmployeeId == command.Level2ApproverEmployeeId)
        {
            throw new InvalidOperationException("Level1 and Level2 approvers must be different.");
        }

        var hierarchy = await _dbContext.ReportingHierarchies
            .SingleOrDefaultAsync(x => x.EmployeeId == command.EmployeeId, cancellationToken);

        var oldLevel1 = hierarchy?.Level1ApproverEmployeeId;
        var oldLevel2 = hierarchy?.Level2ApproverEmployeeId;

        if (hierarchy is null)
        {
            hierarchy = new ReportingHierarchy
            {
                Id = Guid.NewGuid(),
                EmployeeId = command.EmployeeId,
                Level1ApproverEmployeeId = command.Level1ApproverEmployeeId,
                Level2ApproverEmployeeId = command.Level2ApproverEmployeeId
            };

            await _dbContext.ReportingHierarchies.AddAsync(hierarchy, cancellationToken);
        }
        else
        {
            hierarchy.Level1ApproverEmployeeId = command.Level1ApproverEmployeeId;
            hierarchy.Level2ApproverEmployeeId = command.Level2ApproverEmployeeId;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _appCache.RemoveAsync(CacheKeys.Hierarchy(command.EmployeeId), cancellationToken);
        await _appCache.RemoveAsync(CacheKeys.EmployeeDashboard(command.EmployeeId), cancellationToken);

        var managerIds = new[]
        {
            oldLevel1,
            oldLevel2,
            command.Level1ApproverEmployeeId,
            command.Level2ApproverEmployeeId
        }
        .Where(x => x.HasValue)
        .Select(x => x!.Value)
        .Distinct();

        foreach (var managerId in managerIds)
        {
            await _appCache.RemoveAsync(CacheKeys.ManagerDashboard(managerId), cancellationToken);
        }

        return new HierarchyDto(hierarchy.EmployeeId, hierarchy.Level1ApproverEmployeeId, hierarchy.Level2ApproverEmployeeId);
    }

    public async Task<HierarchyDto?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Hierarchy(employeeId);
        var cached = await _appCache.GetAsync<HierarchyDto>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var hierarchy = await _dbContext.ReportingHierarchies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.EmployeeId == employeeId, cancellationToken);

        if (hierarchy is null)
        {
            return null;
        }

        var result = new HierarchyDto(hierarchy.EmployeeId, hierarchy.Level1ApproverEmployeeId, hierarchy.Level2ApproverEmployeeId);
        await _appCache.SetAsync(cacheKey, result, CacheTtls.Hierarchy, cancellationToken);

        return result;
    }
}
