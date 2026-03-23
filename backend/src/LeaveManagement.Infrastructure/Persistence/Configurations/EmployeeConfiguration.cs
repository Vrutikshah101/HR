using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("char(36)");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("char(36)");

        builder.Property(x => x.EmployeeCode)
            .HasColumnName("employee_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Department)
            .HasColumnName("department")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Designation)
            .HasColumnName("designation")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasIndex(x => x.EmployeeCode)
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
