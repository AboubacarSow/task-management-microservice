


namespace task_service.Tests.Fixtures;
public class DatabaseFixture
{
    public IMongoDatabase Database{get;}

    public DatabaseFixture()
    {
        BsonSerializer.RegisterSerializer(
        new GuidSerializer(GuidRepresentation.Standard));

        var client = new MongoClient("mongodb://localhost:27017");
        Database = client.GetDatabase("TaskDbTest");
        Database.DropCollection("Test-Tasks");
        
    }

    public IMongoCollection<TaskItem> GetTaskCollection()
    => Database.GetCollection<TaskItem>("Test-Tasks");

    public async Task Dispose()
    {
        await DropTaskCollectionAsync();
    }



    private async Task DropTaskCollectionAsync()
     => await Database.DropCollectionAsync("Test-Tasks");
}

[CollectionDefinition("MongoDb")]
public class DatabaseDbCollection: ICollectionFixture<DatabaseFixture>
{

}