using IAMUAYTHAI.Domain.Aggregates.ScheduleAggregate;
using IAMUAYTHAI.Domain.Aggregates.TeacherAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAMUAYTHAI.Infra.Configurations
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.DateTime)
                .IsRequired()
                .HasColumnType("datetime2");
                
            builder.Property(s => s.Description)
                .HasMaxLength(500);
                
            builder.Property(s => s.TeacherId)
                .IsRequired();
            
            builder.HasOne<Teacher>()
                .WithMany(t => t.Schedules)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasIndex(s => s.TeacherId)
                .HasDatabaseName("IX_Schedule_TeacherId");
                
            builder.HasIndex(s => s.DateTime)
                .HasDatabaseName("IX_Schedule_DateTime");
                
            builder.HasIndex(s => new { s.TeacherId, s.DateTime })
                .HasDatabaseName("IX_Schedule_TeacherId_DateTime");
        }
    }
}