using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

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
        
    }

    public IMongoCollection<Project> GetProjectCollection()
    => Database.GetCollection<Project>("Test-Projects");


    public async Task Dispose()
    {
        await DropProjectCollectionAsync();
    }

    

    private async Task DropProjectCollectionAsync()
       => await Database.DropCollectionAsync("Test-Projects");


}

[CollectionDefinition("MongoDb")]
public class DatabaseDbCollection: ICollectionFixture<DatabaseFixture>
{

}
