using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGProcessor.Middleware
{
    public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeaderName = "X-API-Key";
    
    public ApiKeyAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        // Skip authentication for health checks or specific paths
        if (context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/ping") ||
            context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/index.html")
            )
        {
            await _next(context);
            return;
        }
        
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }
        
        var validApiKey = configuration["API:Key"];
        
        if (string.IsNullOrEmpty(validApiKey))
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("API Key not configured");
            return;
        }
        
        if (!validApiKey.Equals(potentialApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }
        
        await _next(context);
    }
}
}