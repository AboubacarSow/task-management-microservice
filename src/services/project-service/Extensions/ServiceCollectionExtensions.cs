<<<<<<< HEAD
using Carter;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using project_service.Commons.Behaviors;
using project_service.Data.Repositories;
using project_service.Data.Utilities;
using project_service.Middlewares;
using project_service.Projects.Grpc.Client;
namespace project_service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseCollections(this IServiceCollection services)
    {
        services.AddSingleton<IMongoCollection<Project>>(scope =>
        {
            var settings = scope.GetRequiredService<IOptions<DatabaseSettings>>().Value;

            var client = new MongoClient(settings.ConnectionStrings);
            var database = client.GetDatabase(settings.Database);
            return database.GetCollection<Project>(settings.ProjectCollection);
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
        services.AddScoped<IProjectRepository,ProjectRepository>();
        services.AddProblemDetails();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddTransient<TaskItemClient>();
        return services;
    }
=======
using Carter;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using project_service.Commons.Behaviors;
using project_service.Data.Repositories;
using project_service.Data.Utilities;
using project_service.Middlewares;
using project_service.Projects.Grpc.Client;
namespace project_service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseCollections(this IServiceCollection services)
    {
        services.AddSingleton<IMongoCollection<Project>>(scope =>
        {
            var settings = scope.GetRequiredService<IOptions<DatabaseSettings>>().Value;

            var client = new MongoClient(settings.ConnectionStrings);
            var database = client.GetDatabase(settings.Database);
            return database.GetCollection<Project>(settings.ProjectCollection);
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
        services.AddScoped<IProjectRepository,ProjectRepository>();
        services.AddProblemDetails();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddTransient<TaskItemClient>();
        return services;
    }
>>>>>>> 05b451b (new_update)
}