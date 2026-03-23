using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Infrastructure.Persistence.Configurations;

public class ReportingHierarchyConfiguration : IEntityTypeConfiguration<ReportingHierarchy>
{
    public void Configure(EntityTypeBuilder<ReportingHierarchy> builder)
    {
        builder.ToTable("reporting_hierarchies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("char(36)");

        builder.Property(x => x.EmployeeId)
            .HasColumnName("employee_id")
            .HasColumnType("char(36)");

        builder.Property(x => x.Level1ApproverEmployeeId)
            .HasColumnName("level1_approver_employee_id")
            .HasColumnType("char(36)");

        builder.Property(x => x.Level2ApproverEmployeeId)
            .HasColumnName("level2_approver_employee_id")
            .HasColumnType("char(36)");

        builder.HasIndex(x => x.EmployeeId)
            .IsUnique();

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(x => x.Level1ApproverEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(x => x.Level2ApproverEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
