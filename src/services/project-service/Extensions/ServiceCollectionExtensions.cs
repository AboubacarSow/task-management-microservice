using Carter;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using project_service.Data.Repositories;
using project_service.Data.Utilities;
using project_service.Projects.Models;
using Task = project_service.Tasks.Models.Task;
namespace project_service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCollections(this IServiceCollection services)
    {
        services.AddSingleton<IMongoCollection<Project>>(scope =>
        {
            var settings = scope.GetRequiredService<IOptions<DatabaseSettings>>().Value;

            var client = new MongoClient(settings.ConnectionStrings);
            var database = client.GetDatabase(settings.Database);
            return database.GetCollection<Project>(settings.ProjectCollection);
        });

        services.AddSingleton<IMongoCollection<Task>>(scope =>
        {
            var settings = scope.GetRequiredService<IOptions<DatabaseSettings>>().Value;

            var client = new MongoClient(settings.ConnectionStrings);
            var database = client.GetDatabase(settings.Database);
            return database.GetCollection<Task>(settings.TaskCollection);
        });

        services.AddValidatorsFromAssembly(typeof(AsssemblyReference).Assembly);
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(AsssemblyReference).Assembly);
        });
        services.AddCarter();
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectRepository,ProjectRepository>();
        services.AddScoped<ITaskRepository,TaskRepository>();
        return services;
    }
}