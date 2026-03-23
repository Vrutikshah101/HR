using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Infrastructure.Persistence.Configurations;

public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.ToTable("leave_balances");

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

        builder.Property(x => x.OpeningBalance)
            .HasColumnName("opening_balance")
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Property(x => x.Used)
            .HasColumnName("used")
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Property(x => x.Adjustments)
            .HasColumnName("adjustments")
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Ignore(x => x.Available);

        builder.HasIndex(x => new { x.EmployeeId, x.LeaveTypeCode })
            .IsUnique();

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
