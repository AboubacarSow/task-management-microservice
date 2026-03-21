using MongoDB.Driver;
using TaskItem = project_service.Tasks.Models.TaskItem;
using project_service.Projects.Models;
namespace project_service.Extensions;

public static class ApplicatinBuilderExtensions
{
    public static async Task<IApplicationBuilder> CreateTaskIndexesAync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var taskCollection = scope.ServiceProvider
            .GetRequiredService<IMongoCollection<TaskItem>>();

        var indexes = new List<CreateIndexModel<TaskItem>>
        {
            new (Builders<TaskItem>.IndexKeys.Ascending(t=>t.ProjectId)),
            new (Builders<TaskItem>.IndexKeys.Ascending(t=>t.CreatedByUser)),
            new (Builders<TaskItem>.IndexKeys.Ascending(t=>t.ProjectId)
            .Descending(t=>t.CreatedAt))

        };

        await taskCollection.Indexes.CreateManyAsync(indexes);
        return app;
    }

    public static async Task<IApplicationBuilder> CreateProjectIndexesAync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var projectCollection = scope.ServiceProvider
        .GetRequiredService<IMongoCollection<Project>>();

        var indexes = new List<CreateIndexModel<Project>>{
            new (Builders<Project>.IndexKeys.Ascending(p=>p.CreatedByUser)),
            new (Builders<Project>.IndexKeys.Ascending(p=>p.CreatedByUser)
            .Descending(p=>p.CreatedAt))
        };

        await projectCollection.Indexes.CreateManyAsync(indexes);

        return app;
    }
}
