using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Masters;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Infrastructure.Caching;
using LeaveManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Services;

public class MasterDataService : IMasterDataService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IAppCache _appCache;

    public MasterDataService(ApplicationDbContext dbContext, IAppCache appCache)
    {
        _dbContext = dbContext;
        _appCache = appCache;
    }

    public async Task<IReadOnlyCollection<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.MastersDepartments();
        var cached = await _appCache.GetAsync<DepartmentDto[]>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var rows = await _dbContext.Departments.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new DepartmentDto(x.Id, x.Code, x.Name, x.IsActive))
            .ToArrayAsync(cancellationToken);

        await _appCache.SetAsync(cacheKey, rows, CacheTtls.Masters, cancellationToken);
        return rows;
    }

    public async Task<IReadOnlyCollection<DesignationDto>> GetDesignationsAsync(CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.MastersDesignations();
        var cached = await _appCache.GetAsync<DesignationDto[]>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var rows = await _dbContext.Designations.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new DesignationDto(x.Id, x.Code, x.Name, x.IsActive))
            .ToArrayAsync(cancellationToken);

        await _appCache.SetAsync(cacheKey, rows, CacheTtls.Masters, cancellationToken);
        return rows;
    }

    public async Task<IReadOnlyCollection<DesignationDto>> GetDesignationsByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.DesignationsByDepartment(departmentId);
        var cached = await _appCache.GetAsync<DesignationDto[]>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var rows = await _dbContext.DepartmentDesignationMaps.AsNoTracking()
            .Where(x => x.DepartmentId == departmentId)
            .Join(
                _dbContext.Designations.AsNoTracking(),
                map => map.DesignationId,
                designation => designation.Id,
                (map, designation) => new DesignationDto(designation.Id, designation.Code, designation.Name, designation.IsActive))
            .Distinct()
            .OrderBy(x => x.Name)
            .ToArrayAsync(cancellationToken);

        await _appCache.SetAsync(cacheKey, rows, CacheTtls.Masters, cancellationToken);
        return rows;
    }

    public async Task<IReadOnlyCollection<DepartmentDesignationMapDto>> GetDepartmentDesignationMapsAsync(CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.MastersDepartmentDesignationMap();
        var cached = await _appCache.GetAsync<DepartmentDesignationMapDto[]>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var rows = await _dbContext.DepartmentDesignationMaps.AsNoTracking()
            .Join(
                _dbContext.Departments.AsNoTracking(),
                map => map.DepartmentId,
                department => department.Id,
                (map, department) => new { map, department })
            .Join(
                _dbContext.Designations.AsNoTracking(),
                row => row.map.DesignationId,
                designation => designation.Id,
                (row, designation) => new DepartmentDesignationMapDto(
                    row.map.Id,
                    row.department.Id,
                    row.department.Name,
                    designation.Id,
                    designation.Name))
            .OrderBy(x => x.DepartmentName)
            .ThenBy(x => x.DesignationName)
            .ToArrayAsync(cancellationToken);

        await _appCache.SetAsync(cacheKey, rows, CacheTtls.Masters, cancellationToken);
        return rows;
    }

    public async Task<IReadOnlyCollection<LeaveTypeMasterDto>> GetLeaveTypesAsync(CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.MastersLeaveTypes();
        var cached = await _appCache.GetAsync<LeaveTypeMasterDto[]>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var rows = await _dbContext.LeaveTypes.AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new LeaveTypeMasterDto(x.Id, x.Code, x.Name, x.RequiresAttachment, x.IsPaid, x.IsHalfDayAllowed, x.IsBackdatedAllowed, x.MaxDaysPerRequest, x.IsActive))
            .ToArrayAsync(cancellationToken);

        await _appCache.SetAsync(cacheKey, rows, CacheTtls.Masters, cancellationToken);
        return rows;
    }

    public async Task<IReadOnlyCollection<HolidayDto>> GetHolidaysAsync(CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.MastersHolidays();
        var cached = await _appCache.GetAsync<HolidayDto[]>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var rows = await _dbContext.Holidays.AsNoTracking()
            .OrderBy(x => x.Date)
            .Select(x => new HolidayDto(x.Id, x.Name, x.Date, x.Location, x.IsOptional))
            .ToArrayAsync(cancellationToken);

        await _appCache.SetAsync(cacheKey, rows, CacheTtls.Masters, cancellationToken);
        return rows;
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(string code, string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Department code and name are required.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (await _dbContext.Departments.AnyAsync(x => x.Code == normalizedCode, cancellationToken))
        {
            throw new InvalidOperationException("Department code already exists.");
        }

        var entity = new Department
        {
            Id = Guid.NewGuid(),
            Code = normalizedCode,
            Name = name.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        await _dbContext.Departments.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateMasterCacheAsync(cancellationToken);

        return new DepartmentDto(entity.Id, entity.Code, entity.Name, entity.IsActive);
    }

    public async Task<DesignationDto> CreateDesignationAsync(string code, string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Designation code and name are required.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (await _dbContext.Designations.AnyAsync(x => x.Code == normalizedCode, cancellationToken))
        {
            throw new InvalidOperationException("Designation code already exists.");
        }

        var entity = new Designation
        {
            Id = Guid.NewGuid(),
            Code = normalizedCode,
            Name = name.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        await _dbContext.Designations.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await InvalidateMasterCacheAsync(cancellationToken);

        return new DesignationDto(entity.Id, entity.Code, entity.Name, entity.IsActive);
    }

    public async Task<DepartmentDesignationMapDto> CreateDepartmentDesignationMapAsync(Guid departmentId, Guid designationId, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == departmentId, cancellationToken);
        if (department is null)
        {
            throw new InvalidOperationException("Department not found.");
        }

        var designation = await _dbContext.Designations.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == designationId, cancellationToken);
        if (designation is null)
        {
            throw new InvalidOperationException("Designation not found.");
        }

        var exists = await _dbContext.DepartmentDesignationMaps.AnyAsync(
            x => x.DepartmentId == departmentId && x.DesignationId == designationId,
            cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("Department-designation mapping already exists.");
        }

        var entity = new DepartmentDesignationMap
        {
            Id = Guid.NewGuid(),
            DepartmentId = departmentId,
            DesignationId = designationId,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await _dbContext.DepartmentDesignationMaps.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _appCache.RemoveAsync(CacheKeys.MastersDepartmentDesignationMap(), cancellationToken);
        await _appCache.RemoveAsync(CacheKeys.DesignationsByDepartment(departmentId), cancellationToken);

        return new DepartmentDesignationMapDto(entity.Id, department.Id, department.Name, designation.Id, designation.Name);
    }

    public async Task<LeaveTypeMasterDto> CreateLeaveTypeAsync(string code, string name, bool requiresAttachment, bool isPaid, bool isHalfDayAllowed, bool isBackdatedAllowed, decimal? maxDaysPerRequest, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Leave type code and name are required.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (await _dbContext.LeaveTypes.AnyAsync(x => x.Code == normalizedCode, cancellationToken))
        {
            throw new InvalidOperationException("Leave type code already exists.");
        }

        var entity = new LeaveTypeMaster
        {
            Id = Guid.NewGuid(),
            Code = normalizedCode,
            Name = name.Trim(),
            RequiresAttachment = requiresAttachment,
            IsPaid = isPaid,
            IsHalfDayAllowed = isHalfDayAllowed,
            IsBackdatedAllowed = isBackdatedAllowed,
            MaxDaysPerRequest = maxDaysPerRequest,
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        await _dbContext.LeaveTypes.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _appCache.RemoveAsync(CacheKeys.MastersLeaveTypes(), cancellationToken);

        return new LeaveTypeMasterDto(entity.Id, entity.Code, entity.Name, entity.RequiresAttachment, entity.IsPaid, entity.IsHalfDayAllowed, entity.IsBackdatedAllowed, entity.MaxDaysPerRequest, entity.IsActive);
    }

    public async Task<HolidayDto> CreateHolidayAsync(string name, DateOnly date, string? location, bool isOptional, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Holiday name is required.");
        }

        var existing = await _dbContext.Holidays.AnyAsync(x => x.Date == date && x.Location == location, cancellationToken);
        if (existing)
        {
            throw new InvalidOperationException("Holiday already exists for this date/location.");
        }

        var entity = new Holiday
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Date = date,
            Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim(),
            IsOptional = isOptional,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await _dbContext.Holidays.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _appCache.RemoveAsync(CacheKeys.MastersHolidays(), cancellationToken);

        return new HolidayDto(entity.Id, entity.Name, entity.Date, entity.Location, entity.IsOptional);
    }

    private async Task InvalidateMasterCacheAsync(CancellationToken cancellationToken)
    {
        await _appCache.RemoveAsync(CacheKeys.MastersDepartments(), cancellationToken);
        await _appCache.RemoveAsync(CacheKeys.MastersDesignations(), cancellationToken);
        await _appCache.RemoveAsync(CacheKeys.MastersDepartmentDesignationMap(), cancellationToken);
        await _appCache.RemoveByPrefixAsync("lms:masters:designations-by-department:", cancellationToken);
    }
}
