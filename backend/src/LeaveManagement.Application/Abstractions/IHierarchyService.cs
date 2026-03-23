using LeaveManagement.Application.Hierarchy;

namespace LeaveManagement.Application.Abstractions;

public interface IHierarchyService
{
    Task<HierarchyDto> UpsertAsync(UpsertHierarchyCommand command, CancellationToken cancellationToken);
    Task<HierarchyDto?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken);
}
