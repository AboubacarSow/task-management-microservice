using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace shared.messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransitWitAssembly(this IServiceCollection services,
    IConfiguration configuration,  Assembly assemblyReference)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            //This can change in prod- the storage is not persistante because it's done internally(inside the RAM)
            config.SetInMemorySagaRepositoryProvider();

            config.AddConsumers(assemblyReference);

            config.AddSagaStateMachines(assemblyReference);
            config.AddSagas(assemblyReference);
            config.AddActivities(assemblyReference);

           

            config.UsingRabbitMq((context, busConfigurator) =>
            {
                busConfigurator.Host(new Uri(configuration["MessageBroker:Host"]!), host =>
                {
                    host.Username(configuration["MessageBroker:Username"]!);
                    host.Password(configuration["MessageBroker:Password"]!);
                });
                busConfigurator.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}