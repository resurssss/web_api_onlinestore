using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("")]
    public class HealthController : ControllerBase
    {
        private static readonly Counter HealthCheckCounter = Metrics
            .CreateCounter("health_checks_total", "Total number of health checks");
        
        private readonly string _instanceId;

        public HealthController()
        {
            _instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? "Unknown-Instance";
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            HealthCheckCounter.Inc();
            
            // Временно возвращаем 500 для теста
            return StatusCode(500, new { status = "unhealthy", instance = _instanceId });
        }
    }
}