using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace shared.Configurations;

public static class MongoConfiguration
{

    private static int _configured = 0;

    public static void Configure()
    {
        if (Interlocked.Exchange(ref _configured, 1) == 1)
            return;

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

    }
}