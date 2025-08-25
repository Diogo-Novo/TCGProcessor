using TCGProcessor.Data;
using Microsoft.EntityFrameworkCore;
using TCGProcessor.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();


#region CORS Configuration
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" };
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


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (
    app.Environment.IsDevelopment()
    || app.Environment.IsProduction()
    )
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
