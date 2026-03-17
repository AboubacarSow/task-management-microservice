using MongoDB.Driver;
using Task = project_service.Tasks.Models.Task;
namespace project_service.Extensions;

public static class ApplicatinBuilderExtensions
{
    public static async Task<IApplicationBuilder> CreateTaskIndexesAync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var taskCollection = scope.ServiceProvider
            .GetRequiredService<IMongoCollection<Task>>();

        var indexes = new List<CreateIndexModel<Task>>
        {
            new (Builders<Task>.IndexKeys.Ascending(t=>t.ProjectId)),
            new (Builders<Task>.IndexKeys.Ascending(t=>t.ProjectId)
            .Descending(t=>t.CreatedAt))

        };

        await taskCollection.Indexes.CreateManyAsync(indexes);
        return app;
    }
}
