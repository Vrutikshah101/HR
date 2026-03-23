using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Infrastructure.Persistence.Configurations;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("leave_requests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("char(36)");

        builder.Property(x => x.EmployeeId)
            .HasColumnName("employee_id")
            .HasColumnType("char(36)");

        builder.Property(x => x.LeaveTypeCode)
            .HasColumnName("leave_type_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(x => x.EndDate)
            .HasColumnName("end_date")
            .IsRequired();

        builder.Property(x => x.Days)
            .HasColumnName("days")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasColumnName("reason")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
