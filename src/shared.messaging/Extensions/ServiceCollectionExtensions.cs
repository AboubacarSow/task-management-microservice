using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shared.messaging.Utils;
using System.Reflection;

namespace shared.messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransitWithAssembly(
    this IServiceCollection services,
    IConfiguration configuration,
    Assembly assemblyReference)
    {

        services.Configure<MessageBrokerSettings>(configuration.GetSection(nameof(MessageBrokerSettings)));
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.AddConsumers(assemblyReference);
            config.AddSagaStateMachines(assemblyReference);
            config.AddSagas(assemblyReference);
            config.AddActivities(assemblyReference);

            config.UsingRabbitMq((context, busConfigurator) =>
            {
            var messageBroker = configuration.GetSection("MessageBroker")
                        .Get<MessageBrokerSettings>();
                
                busConfigurator.Host(messageBroker!.Host, "/", host =>
                {
                    host.Username(messageBroker.Username);
                    host.Password(messageBroker.Password);
                });
                busConfigurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}