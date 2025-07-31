using IAMUAYTHAI.Domain.Aggregates.StudentClassAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAMUAYTHAI.Infra.Configurations
{
    internal class StudentClassConfigurations : IEntityTypeConfiguration<StudentClass>
    {
        public void Configure(EntityTypeBuilder<StudentClass> builder)
        {
            builder.HasKey(sc => sc.Id);

            builder.HasOne(sc => sc.Student)
                .WithMany(s => s.StudentClasses)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sc => sc.Class)
                .WithMany(c => c.StudentClasses)
                .HasForeignKey(sc => sc.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(sc => sc.WasPresent)
                .IsRequired();

            builder.Property(sc => sc.Justification)
                .HasMaxLength(500);

            // Índice único para evitar duplicatas
            builder.HasIndex(sc => new { sc.StudentId, sc.ClassId })
                .IsUnique()
                .HasDatabaseName("IX_StudentClass_Student_Class");
        }
    }
}