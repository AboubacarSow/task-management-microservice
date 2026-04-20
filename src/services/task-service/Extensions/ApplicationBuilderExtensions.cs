<<<<<<< HEAD
namespace task_service.Extensions;

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

}
=======
namespace task_service.Extensions;

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

}
>>>>>>> 05b451b (new_update)
