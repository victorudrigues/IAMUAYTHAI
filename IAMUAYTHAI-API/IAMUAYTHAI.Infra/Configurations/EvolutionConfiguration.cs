using IAMUAYTHAI.Domain.Aggregates.EvolutionAggregate;
using IAMUAYTHAI.Domain.Aggregates.StudentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAMUAYTHAI.Infra.Configurations
{
    public class EvolutionConfiguration : IEntityTypeConfiguration<Evolution>
    {
        public void Configure(EntityTypeBuilder<Evolution> builder)
        {
            // Chave primária
            builder.HasKey(e => e.Id);
            
            // Propriedades
            builder.Property(e => e.StudentId)
                .IsRequired();
                
            builder.Property(e => e.CurrentLevel)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(e => e.NextLevel)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(e => e.NextKruangExpectedDate)
                .IsRequired()
                .HasColumnType("datetime2");
                
            builder.Property(e => e.EligibleForNextLevel)
                .IsRequired();
            
            // Relacionamento One-to-One com Student
            builder.HasOne<Student>()
                .WithOne(s => s.CurrentEvolution)
                .HasForeignKey<Evolution>(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Índice único no StudentId (garante one-to-one)
            builder.HasIndex(e => e.StudentId)
                .IsUnique()
                .HasDatabaseName("IX_Evolution_StudentId_Unique");
                
            // Índice na data esperada para consultas de cronograma
            builder.HasIndex(e => e.NextKruangExpectedDate)
                .HasDatabaseName("IX_Evolution_NextKruangExpectedDate");
                
            // Índice no nível atual para relatórios
            builder.HasIndex(e => e.CurrentLevel)
                .HasDatabaseName("IX_Evolution_CurrentLevel");
        }
    }
}