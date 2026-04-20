using MongoDB.Driver;
namespace project_service.Extensions;

public static class ApplicatinBuilderExtensions
{
    public static async Task<IApplicationBuilder> CreateProjectIndexesAync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var projectCollection = scope.ServiceProvider
        .GetRequiredService<IMongoCollection<Project>>();

        var indexes = new List<CreateIndexModel<Project>>{
            new (Builders<Project>.IndexKeys.Ascending(p=>p.OwnerId)),
            new (Builders<Project>.IndexKeys.Ascending(p=>p.OwnerId)
            .Descending(p=>p.CreatedAt))
        };

        

        await projectCollection.Indexes.CreateManyAsync(indexes);

        return app;
    }
}
