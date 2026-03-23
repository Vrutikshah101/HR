using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserRoleAssignment> UserRoles => Set<UserRoleAssignment>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ReportingHierarchy> ReportingHierarchies => Set<ReportingHierarchy>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveRequestApproval> LeaveRequestApprovals => Set<LeaveRequestApproval>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
