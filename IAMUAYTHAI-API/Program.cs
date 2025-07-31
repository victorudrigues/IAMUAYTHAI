using IAMUAYTHAI.Infra;
using IAMUAYTHAI_API.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra o migrator no DI
builder.Services.AddScoped<IContextMigrator, ContextMigrator>();
builder.Services.AddFeaturesServices();

// Adicione o DbContext antes de builder.Build()
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("IAMUAYTHAI.Infra").MigrationsHistoryTable("__EFMigrationsHistory", "dbo")
    )
);

var app = builder.Build();

// Aplica as migrations usando o ContextMigrator
using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<IContextMigrator>();
    migrator.MigrateAsync().GetAwaiter().GetResult();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();