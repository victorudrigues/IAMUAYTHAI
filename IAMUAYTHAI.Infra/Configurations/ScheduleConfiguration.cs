using IAMUAYTHAI.Domain.Aggregates.ScheduleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAMUAYTHAI.Infra.Configurations
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Description).HasMaxLength(200);
            builder.Property(s => s.DateTime).IsRequired();
        }
    }
}