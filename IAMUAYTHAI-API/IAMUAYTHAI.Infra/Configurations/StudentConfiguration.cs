using IAMUAYTHAI.Domain.Aggregates.StudentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAMUAYTHAI.Infra.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // Propriedades específicas do Student
            builder.Property(s => s.BirthDate)
                .IsRequired()
                .HasColumnType("datetime2");
                
            // Índices para performance
            builder.HasIndex(s => s.BirthDate)
                .HasDatabaseName("IX_Student_BirthDate");
        }
    }
}