using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Infrastructure.Persistence.Configurations;

public class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(x => new { x.UserId, x.RoleCode });

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("char(36)");

        builder.Property(x => x.RoleCode)
            .HasColumnName("role_code")
            .HasConversion<int>()
            .IsRequired();
    }
}
