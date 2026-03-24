using LeaveManagement.Application.Masters;

namespace LeaveManagement.Application.Abstractions;

public interface IMasterDataService
{
    Task<IReadOnlyCollection<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DesignationDto>> GetDesignationsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DesignationDto>> GetDesignationsByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DepartmentDesignationMapDto>> GetDepartmentDesignationMapsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LeaveTypeMasterDto>> GetLeaveTypesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<HolidayDto>> GetHolidaysAsync(CancellationToken cancellationToken);

    Task<DepartmentDto> CreateDepartmentAsync(string code, string name, CancellationToken cancellationToken);
    Task<DesignationDto> CreateDesignationAsync(string code, string name, CancellationToken cancellationToken);
    Task<DepartmentDesignationMapDto> CreateDepartmentDesignationMapAsync(Guid departmentId, Guid designationId, CancellationToken cancellationToken);
    Task<LeaveTypeMasterDto> CreateLeaveTypeAsync(string code, string name, bool requiresAttachment, bool isPaid, bool isHalfDayAllowed, bool isBackdatedAllowed, decimal? maxDaysPerRequest, CancellationToken cancellationToken);
    Task<HolidayDto> CreateHolidayAsync(string name, DateOnly date, string? location, bool isOptional, CancellationToken cancellationToken);
}
