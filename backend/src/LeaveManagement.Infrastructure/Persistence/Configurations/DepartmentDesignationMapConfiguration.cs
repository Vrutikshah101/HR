using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Infrastructure.Persistence.Configurations;

public class DepartmentDesignationMapConfiguration : IEntityTypeConfiguration<DepartmentDesignationMap>
{
    public void Configure(EntityTypeBuilder<DepartmentDesignationMap> builder)
    {
        builder.ToTable("department_designation_maps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("char(36)");

        builder.Property(x => x.DepartmentId)
            .HasColumnName("department_id")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.Property(x => x.DesignationId)
            .HasColumnName("designation_id")
            .HasColumnType("char(36)")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(x => new { x.DepartmentId, x.DesignationId })
            .IsUnique();
    }
}
