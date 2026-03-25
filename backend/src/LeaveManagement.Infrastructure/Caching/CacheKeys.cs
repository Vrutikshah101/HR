namespace LeaveManagement.Infrastructure.Caching;

public static class CacheKeys
{
    public static string MastersDepartments() => "lms:masters:departments:all";
    public static string MastersDesignations() => "lms:masters:designations:all";
    public static string MastersDepartmentDesignationMap() => "lms:masters:department-designation-map:all";
    public static string MastersLeaveTypes() => "lms:masters:leave-types:all";
    public static string MastersHolidays() => "lms:masters:holidays:all";
    public static string DesignationsByDepartment(Guid departmentId) => $"lms:masters:designations-by-department:{departmentId:N}";

    public static string UserProfile(Guid userId) => $"lms:user:profile:{userId:N}";
    public static string Hierarchy(Guid employeeId) => $"lms:hierarchy:{employeeId:N}";

    public static string EmployeeDashboard(Guid employeeId) => $"lms:dashboard:employee:{employeeId:N}";
    public static string ManagerDashboard(Guid employeeId) => $"lms:dashboard:manager:{employeeId:N}";
    public static string HrDashboard() => "lms:dashboard:hr:all";
    public static string AdminDashboard() => "lms:dashboard:admin:all";

    public static string LeaveBalances(Guid employeeId) => $"lms:leave-balances:{employeeId:N}";
}
