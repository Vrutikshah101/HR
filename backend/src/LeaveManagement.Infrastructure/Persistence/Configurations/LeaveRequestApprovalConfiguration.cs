using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Infrastructure.Persistence.Configurations;

public class LeaveRequestApprovalConfiguration : IEntityTypeConfiguration<LeaveRequestApproval>
{
    public void Configure(EntityTypeBuilder<LeaveRequestApproval> builder)
    {
        builder.ToTable("leave_request_approvals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("char(36)");

        builder.Property(x => x.LeaveRequestId)
            .HasColumnName("leave_request_id")
            .HasColumnType("char(36)");

        builder.Property(x => x.ApprovalLevel)
            .HasColumnName("approval_level")
            .IsRequired();

        builder.Property(x => x.ApproverEmployeeId)
            .HasColumnName("approver_employee_id")
            .HasColumnType("char(36)");

        builder.Property(x => x.Action)
            .HasColumnName("action")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasColumnName("comment")
            .HasMaxLength(1000);

        builder.Property(x => x.ActionedAtUtc)
            .HasColumnName("actioned_at_utc")
            .IsRequired();

        builder.HasOne<LeaveRequest>()
            .WithMany()
            .HasForeignKey(x => x.LeaveRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(x => x.ApproverEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
