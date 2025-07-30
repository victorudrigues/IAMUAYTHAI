using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IAMUAYTHAI.Infra
{
    public interface IContextMigrator
    {
        Task MigrateAsync(CancellationToken cancellationToken = default);
    }

    public class ContextMigrator(Context context) : IContextMigrator
    {
        private readonly Context _context = context;

        public async Task MigrateAsync(CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await ((IMigrator)_context.GetService(typeof(IMigrator))).MigrateAsync(null, cancellationToken);
            });
        }
    }
}
