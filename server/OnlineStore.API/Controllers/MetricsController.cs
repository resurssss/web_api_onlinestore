using Microsoft.AspNetCore.Mvc;
using Prometheus;
using System.Text;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("")]
    public class MetricsController : ControllerBase
    {
        private readonly string _instanceId;

        public MetricsController()
        {
            _instanceId = Environment.GetEnvironmentVariable("INSTANCE_ID") ?? "Unknown-Instance";
        }

        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics()
        {
            Response.Headers.Append("X-Instance-Id", _instanceId);
            
            // Собираем все метрики
            var registry = Metrics.DefaultRegistry;
            var stream = new MemoryStream();
            await registry.CollectAndExportAsTextAsync(stream);
            stream.Position = 0;
            
            using var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();
            
            return Content(text, "text/plain; version=0.0.4; charset=utf-8");
        }
    }
}