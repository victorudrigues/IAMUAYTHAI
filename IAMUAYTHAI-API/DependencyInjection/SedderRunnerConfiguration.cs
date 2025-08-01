using IAMUAYTHAI.Infra;
using Microsoft.EntityFrameworkCore;

namespace IAMUAYTHAI_API.DependencyInjection
{
    public static class SedderRunnerConfiguration
    {
        public static async Task ExecuteAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var migrator = scope.ServiceProvider.GetRequiredService<IContextMigrator>();

            await migrator.MigrateAsync();

            try
            {
                var passwordService = scope.ServiceProvider.GetRequiredService<
                    IAMUAYTHAI.Application.Abstractions.Features.Auth.Services.IPasswordHashService>();

                if (!await context.Users.AnyAsync(u => u.Profile == IAMUAYTHAI.Domain.Enumerations.UserProfileType.Admin))
                {
                    var user = new IAMUAYTHAI.Domain.Aggregates.UserAggregate.User
                    {
                        Name = "Administrador",
                        Email = "admin@iamuaythai.com",
                        PasswordHash = passwordService.HashPassword("Admin@14@43MuayThai"),
                        Profile = IAMUAYTHAI.Domain.Enumerations.UserProfileType.Admin
                    };

                    context.Users.Add(user);
                    await context.SaveChangesAsync();

                    Console.WriteLine($"✅ Admin criado: {user.Email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erro ao aplicar seed: {ex.Message}");
            }
        }
    }
}
