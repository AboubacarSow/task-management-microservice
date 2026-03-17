using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using project_service.Data.Repositories;
using project_service.Projects.Models;
using project_service.Tests.Data.Repositories;
using Task = project_service.Tasks.Models.Task;

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
    public IMongoCollection<Task> GetTaskCollection()
    => Database.GetCollection<Task>("Test-Tasks");

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