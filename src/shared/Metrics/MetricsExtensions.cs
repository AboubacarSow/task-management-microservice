<<<<<<< HEAD
using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace shared.Metrics;

public static class MetricsExtensions
{
    public static void UseMetrics(this WebApplication app,string service)
    {
        app.UseHttpMetrics(options =>
        {
            options.AddCustomLabel("service", _ => service);
        });
        
        app.MapMetrics();              
    }
=======
using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace shared.Metrics;

public static class MetricsExtensions
{
    public static void UseMetrics(this WebApplication app,string service)
    {
        app.UseHttpMetrics(options =>
        {
            options.AddCustomLabel("service", _ => service);
        });
        
        app.MapMetrics();              
    }
>>>>>>> 05b451b (new_update)
}