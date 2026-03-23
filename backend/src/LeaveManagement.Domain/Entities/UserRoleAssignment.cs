using LeaveManagement.Domain.Enums;

namespace LeaveManagement.Domain.Entities;

public class UserRoleAssignment
{
    public Guid UserId { get; set; }
    public UserRole RoleCode { get; set; }

    public User? User { get; set; }
}
