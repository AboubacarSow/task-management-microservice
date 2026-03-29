using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace shared.Metrics;

public static class MetricsExtensions
{
    public static void UseMetrics(this WebApplication app)
    {
        app.UseMetricServer();
        app.UseHttpMetrics();
    }
}