using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Infrastructure.Persistence.Configurations;

public class LeaveTypeMasterConfiguration : IEntityTypeConfiguration<LeaveTypeMaster>
{
    public void Configure(EntityTypeBuilder<LeaveTypeMaster> builder)
    {
        builder.ToTable("leave_types");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("char(36)");
        builder.Property(x => x.Code).HasColumnName("leave_code").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasColumnName("leave_name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.RequiresAttachment).HasColumnName("requires_attachment").IsRequired();
        builder.Property(x => x.IsPaid).HasColumnName("is_paid").IsRequired();
        builder.Property(x => x.IsHalfDayAllowed).HasColumnName("is_half_day_allowed").IsRequired();
        builder.Property(x => x.IsBackdatedAllowed).HasColumnName("is_backdated_allowed").IsRequired();
        builder.Property(x => x.MaxDaysPerRequest).HasColumnName("max_days_per_request").HasPrecision(6, 2);
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();
    }
}
