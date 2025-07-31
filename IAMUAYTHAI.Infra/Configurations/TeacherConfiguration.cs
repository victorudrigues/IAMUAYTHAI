using IAMUAYTHAI.Domain.Aggregates.TeacherAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAMUAYTHAI.Infra.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
                      
            builder.HasIndex(t => t.Email)
                .HasDatabaseName("IX_Teacher_Email");
                
            // Índice no nome para busca por nome de professor
            builder.HasIndex(t => t.Name)
                .HasDatabaseName("IX_Teacher_Name");
        }
    }
}