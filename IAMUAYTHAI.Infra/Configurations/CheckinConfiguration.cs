using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAMUAYTHAI.Domain.Aggregates.CheckinAggregate;
using IAMUAYTHAI.Domain.Aggregates.StudentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAMUAYTHAI.Infra.Configurations
{
    public class CheckinConfiguration : IEntityTypeConfiguration<Checkin>
    {
        public void Configure(EntityTypeBuilder<Checkin> builder)
        {
         
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.DateTime)
                .IsRequired()
                .HasColumnType("datetime2");
                
            builder.Property(c => c.StudentId)
                .IsRequired();
                
            // Relacionamento com Student (Many-to-One)
            builder.HasOne<Student>()
                .WithMany(s => s.Checkins)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasIndex(c => c.StudentId)
                .HasDatabaseName("IX_Checkin_StudentId");
                
            builder.HasIndex(c => c.DateTime)
                .HasDatabaseName("IX_Checkin_DateTime");
                
            builder.HasIndex(c => new { c.StudentId, c.DateTime })
                .HasDatabaseName("IX_Checkin_StudentId_DateTime");
        }
    }
}
