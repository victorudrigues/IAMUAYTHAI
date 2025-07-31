using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IAMUAYTHAI.Domain.Aggregates.UserAggregate;

namespace IAMUAYTHAI.Infra.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);
                
            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(u => u.Profile)
                .HasConversion<int>()
                .IsRequired();
                
            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}