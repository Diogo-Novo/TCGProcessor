using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TCGProcessor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public HealthController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous] // If using filter approach, add this attribute
        public IActionResult Get()
        {
            return Ok(
                new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0.0"
                }
            );
        }

        // [HttpGet("debug-config")]
        // public IActionResult DebugConfig()
        // {
        //     var configValues = new
        //     {
        //         ApiKey = Configuration["API:Key"],
        //         PricingConnection = Configuration.GetConnectionString("OS-MGX-PricingSystem"),
        //         ProcessorConnection = Configuration.GetConnectionString("OS-MGX-Processor"),
        //         CorsOrigins = Configuration["Cors:AllowedOrigins"],
        //         AllEnvironmentVariables = Environment.GetEnvironmentVariables()
        //     };

        //     return Ok(configValues);
        // }
    }
}
