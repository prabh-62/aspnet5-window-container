using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PaymentsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Healthy");
        }

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var process = Process.GetCurrentProcess();
            var stats = new
            {
                os = new
                {
                    description = RuntimeInformation.OSDescription,
                    type = Enum.GetName(RuntimeInformation.OSArchitecture),
                    bit64 = Environment.Is64BitOperatingSystem
                },
                framework = RuntimeInformation.FrameworkDescription,
                containerized = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") is object ? "true" : "false",
                user = new
                {
                    username = Environment.UserName
                },
                machine = new
                {
                    name = Environment.MachineName
                },
                cpu = new
                {
                    cores = Environment.ProcessorCount
                },
                memory = new
                {
                    currentUsage = $"{ConvertBytesToMegabytes(process.WorkingSet64)} MB",
                    maxAvailable = $"{ConvertBytesToMegabytes(process.MaxWorkingSet.ToInt64())} MB"
                }
            };

            return Ok(stats);
        }
        private static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}
