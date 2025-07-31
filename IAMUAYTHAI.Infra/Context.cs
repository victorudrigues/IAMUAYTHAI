using IAMUAYTHAI.Application.Abstractions;
using IAMUAYTHAI.Domain.Aggregates.CheckinAggregate;
using IAMUAYTHAI.Domain.Aggregates.ClassAggregate;
using IAMUAYTHAI.Domain.Aggregates.EvolutionAggregate;
using IAMUAYTHAI.Domain.Aggregates.ScheduleAggregate;
using IAMUAYTHAI.Domain.Aggregates.StudentAggregate;
using IAMUAYTHAI.Domain.Aggregates.StudentClassAggregate;
using IAMUAYTHAI.Domain.Aggregates.TeacherAggregate;
using IAMUAYTHAI.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IAMUAYTHAI.Infra
{
    public class Context(DbContextOptions<Context> options) : DbContext(options), IUnitOfWork
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Checkin> Checkins { get; set; }
        public DbSet<Evolution> Evolutions { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }

  
    public class ContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            var connectionString = "Server=localhost;Database=IAMUAYTHAI;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                options.MigrationsAssembly("IAMUAYTHAI.Infra");
                options.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
            });

            return new Context(optionsBuilder.Options);
        }
    }
}
