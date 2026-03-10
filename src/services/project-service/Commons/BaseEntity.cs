using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace project_service.Commons;

public abstract class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public DateTime CreatedAt{ get; protected set; }
    public DateTime LastUpdatedAt { get; protected set; }
    public DateTime? DueAt { get; protected set; }
    public string? Description { get; protected set; }

}