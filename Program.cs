using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using TCGProcessor.Data;
using TCGProcessor.Interfaces;
using TCGProcessor.Middleware;
using TCGProcessor.Repositories;
using TCGProcessor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(10); // Default is 30 seconds
    options.KeepAliveInterval = TimeSpan.FromMinutes(2); // Default is 15 seconds
    options.HandshakeTimeout = TimeSpan.FromMinutes(2); // Default is 15 seconds
});

#region CORS Configuration
var corsOrigins = Environment.GetEnvironmentVariable("CORS__ALLOWED_ORIGINS") 
    ?? builder.Configuration["Cors:AllowedOrigins"];

string[] allowedOrigins;
if (string.IsNullOrEmpty(corsOrigins))
{
    allowedOrigins = new[] { "*" }; // Fallback
}
else
{
    allowedOrigins = corsOrigins
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(origin => origin.Trim().TrimEnd('/'))
        .ToArray();
}

// Debug logging to see what origins are configured
Console.WriteLine($"Configured CORS origins: {string.Join(", ", allowedOrigins)}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("CustomCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
#endregion

#region DB Configuration
//Configure MySQL Database
builder.Services.AddDbContext<OsMgxPricingSystemContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("OS-MGX-PricingSystem"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("OS-MGX-PricingSystem"))
    )
);

builder.Services.AddDbContext<OSMGXProcessorDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("OS-MGX-Processor"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("OS-MGX-Processor"))
    )
);
#endregion

#region Services Configuration
// Configure Rate Limiting
//builder.Services.ConfigureRateLimit(builder.Configuration);

// Configure Caching
builder.Services.AddMemoryCache();

// Register HttpClient for enhanced Scryfall service
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<ManaBoxProcessingService>();

// Register our services
builder.Services.AddScoped<IJsonProcessingService, JsonProcessingService>();
builder.Services.AddScoped<PricingSheetRepository>();
builder.Services.AddScoped<CacheRepository>();
builder.Services.AddSingleton<IJobTracker, JobTrackerService>();
builder.Services.AddScoped<IScryfallService, ScryfallService>();
#endregion
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region Swagger
// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "MGX Processor API",
            Version = "v1.0",
            Description = "Enhanced MGX Processor API with Secure Endpoints",
            Contact = new OpenApiContact
            {
                Name = "MGX Support Team",
                Email = "shop@mobilegamesexchange.co.uk"
            }
        }
    );
});
#endregion


var app = builder.Build();
app.UseCors("CustomCors");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()
// || app.Environment.IsProduction()
)
{
    app.MapOpenApi();
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MGX Processor API V1.0");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ProcessingHub>("/processingHub");

app.Run();
