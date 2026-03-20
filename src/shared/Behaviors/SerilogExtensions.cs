using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace shared.Behaviors;

public static class SerilogExtensions
{
    public static IHostBuilder UseCustomSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName();

            if (!context.HostingEnvironment.IsEnvironment("Test"))
            {
                var elasticUri = context.Configuration["Elasticsearch:Uri"];

                if (!string.IsNullOrEmpty(elasticUri))
                {
                    configuration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        IndexFormat = "taskmanagement-logs-{0:yyyy-MM}",
                        AutoRegisterTemplate = true,
                        NumberOfReplicas = 1,
                        NumberOfShards = 2,
                        BatchAction = ElasticOpType.Create,
                        BatchPostingLimit = 50
                    });
                }
            }
        });
    }
}

