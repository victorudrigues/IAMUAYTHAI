using System.Threading.RateLimiting;
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

// ============= CORS =============
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? (builder.Environment.IsDevelopment() ? new[] { "*" } : Array.Empty<string>());
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        if (allowedOrigins.Length == 1 && allowedOrigins[0] == "*")
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        else
            policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

// ============= RATE LIMITING (anti brute-force no login, por IP) =============
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("Login", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "anon";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            Window = TimeSpan.FromMinutes(1),
            PermitLimit = 5
        });
    });
});

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
app.UseCors("DefaultCors");
app.UseRateLimiter();
app.UseAuthentication();

// Sempre apos Authentication
app.UseMiddleware<TokenBlacklistMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.Run();