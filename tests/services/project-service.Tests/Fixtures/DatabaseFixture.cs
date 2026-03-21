using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using project_service.Projects.Models;
using TaskItem = project_service.Tasks.Models.TaskItem;

namespace project_service.Tests.Fixtures;

public class DatabaseFixture
{
    public IMongoDatabase Database{get;}

    public DatabaseFixture()
    {
        BsonSerializer.RegisterSerializer(
        new GuidSerializer(GuidRepresentation.Standard));

        var client = new MongoClient("mongodb://localhost:27017");
        Database = client.GetDatabase("TaskManagementDbTest");
        Database.DropCollection("Test-Projects");
        Database.DropCollection("Test-Tasks");
        
    }

    public IMongoCollection<Project> GetProjectCollection()
    => Database.GetCollection<Project>("Test-Projects");
    public IMongoCollection<TaskItem> GetTaskCollection()
    => Database.GetCollection<TaskItem>("Test-Tasks");

    public async System.Threading.Tasks.Task Dispose()
    {
        await DropProjectCollectionAsync();
        await DropTaskCollectionAsync();
    }

    

    private async System.Threading.Tasks.Task DropProjectCollectionAsync()
       => await Database.DropCollectionAsync("Test-Projects");

    private async System.Threading.Tasks.Task DropTaskCollectionAsync()
     => await Database.DropCollectionAsync("Test-Tasks");
}

[CollectionDefinition("MongoDb")]
public class DatabaseDbCollection: ICollectionFixture<DatabaseFixture>
{

}
//public class ProgramFixure
//{
//    public static WebApplication CreateTestApp()
//    {
//        var builder = WebApplication.CreateBuilder();        
//        // Test authentication scheme
//        builder.Services.AddAuthentication(options =>
//        {
//            options.DefaultAuthenticateScheme = "Test";
//            options.DefaultChallengeScheme = "Test";
//        })
//        .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", _ => { });

//        builder.Services.AddAuthorization();
//        builder.Services.AddHttpContextAccessor();
//        builder.Services.AddScoped<IUserContext, HttpUserContext>();

//        var app = builder.Build();
//        app.UseAuthentication();
//        app.UseAuthorization();

//        app.MapCarter();
//        return app;
//    }
//}