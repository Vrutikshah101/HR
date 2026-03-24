using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Infrastructure.Persistence.Configurations;

public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.ToTable("holidays");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("char(36)");
        builder.Property(x => x.Name).HasColumnName("holiday_name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Date).HasColumnName("holiday_date").IsRequired();
        builder.Property(x => x.Location).HasColumnName("location").HasMaxLength(100);
        builder.Property(x => x.IsOptional).HasColumnName("is_optional").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();

        builder.HasIndex(x => new { x.Date, x.Location }).IsUnique();
    }
}
