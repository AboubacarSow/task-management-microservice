using Carter;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using task_service.Commons.Behaviors;
using task_service.Data.Repositories;
using task_service.Data.Utilities;
using task_service.Middlewares;
using task_service.Tasks.Grpc.Client;
namespace task_service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseCollections(this IServiceCollection services)
    {
      

        services.AddSingleton<IMongoCollection<TaskItem>>(scope =>
        {
            var settings = scope.GetRequiredService<IOptions<DatabaseSettings>>().Value;

            var client = new MongoClient(settings.ConnectionStrings);
            var database = client.GetDatabase(settings.Database);
            return database.GetCollection<TaskItem>(settings.TaskCollection);
        });

        services.AddValidatorsFromAssembly(typeof(AsssemblyReference).Assembly);
        services.AddMediatR(configuration =>
        {
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.RegisterServicesFromAssembly(typeof(AsssemblyReference).Assembly);
        });
        services.AddCarter();
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskRepository,TaskRepository>();
        services.AddProblemDetails();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddTransient<ProjectClient>();
        return services;
    }
}