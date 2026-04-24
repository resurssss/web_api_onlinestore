using Microsoft.AspNetCore.Mvc;
using Prometheus;
using System.Text;
using System.Text.Json;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("")]
    public class HealthController : ControllerBase
    {
        private static readonly Counter HealthCheckCounter = Metrics
            .CreateCounter("health_checks_total", "Total number of health checks");
        
        private static readonly Histogram ResponseSizeHistogram = Metrics
            .CreateHistogram("http_response_size_bytes", "Response size in bytes",
                new HistogramConfiguration
                {
                    Buckets = new double[] { 100, 500, 1000, 5000, 10000, 50000, 100000 },
                    LabelNames = new[] { "route" }
                });
        
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
            
            var response = new { status = "healthy", instance = _instanceId, version = "v2.0" };
            var json = JsonSerializer.Serialize(response);
            var sizeInBytes = Encoding.UTF8.GetByteCount(json);
            ResponseSizeHistogram.WithLabels("health").Observe(sizeInBytes);
            
            return Ok(response);
        }
    }
}