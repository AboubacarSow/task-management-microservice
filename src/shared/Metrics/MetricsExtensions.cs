using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace shared.Metrics;

public static class MetricsExtensions
{
    public static void UseMetrics(this WebApplication app,string jobName)
    {
        app.UseHttpMetrics(options =>
        {
            options.AddCustomLabel("job", _ => jobName);
        });
        
        app.MapMetrics();              
    }
}