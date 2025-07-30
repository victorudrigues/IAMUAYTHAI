using IAMUAYTHAI.Application.Abstractions;
using IAMUAYTHAI.Domain.Aggregates.CheckinAggregate;
using IAMUAYTHAI.Domain.Aggregates.ClassAggregate;
using IAMUAYTHAI.Domain.Aggregates.EvolutionAggregate;
using IAMUAYTHAI.Domain.Aggregates.ScheduleAggregate;
using IAMUAYTHAI.Domain.Aggregates.StudentAggregate;
using IAMUAYTHAI.Domain.Aggregates.TeacherAggregate;
using IAMUAYTHAI.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
