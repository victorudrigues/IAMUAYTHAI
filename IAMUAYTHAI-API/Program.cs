using IAMUAYTHAI.Application.Abstractions.Options;
using IAMUAYTHAI.Infra;
using IAMUAYTHAI_API.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============= USER SECRETS =============
if (builder.Environment.IsDevelopment())
    builder.Configuration.AddUserSecrets<Program>();

// ============= OPTIONS =============
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

// ============= SERVICES =============
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation(builder.Environment);
builder.Services.AddScoped<IContextMigrator, ContextMigrator>();
builder.Services.AddFeaturesServices();
builder.Services.AddJwtAuthentication(builder.Configuration);

// ============= DB =============
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("IAMUAYTHAI.Infra").MigrationsHistoryTable("__EFMigrationsHistory", "dbo")
    )
);

var app = builder.Build();

// ============= MIGRATION + SEED =============
await SedderRunnerConfiguration.ExecuteAsync(app.Services);

// ============= PIPELINE ============
if (app.Environment.IsDevelopment())
    app.UseSwaggerDocumentation();
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();

// Sempre após Authentication
app.UseMiddleware<TokenBlacklistMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.Run();