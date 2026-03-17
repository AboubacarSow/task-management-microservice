using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using project_service.Commons;

namespace project_service.Projects.Models;

public sealed class Project :BaseEntity
{

    public ProjectStatus Status { get; private set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid CreatedByUser { get;private set;}

    public Project(string name,Guid userId, string? description =null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name can not be empty");
        if(userId.Equals(Guid.Empty)) 
            throw new ArgumentException("Project cannot be created without its [CreateByUser] specified");

        if (description is not null && string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Project description can not be empty");

        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = CreatedAt;
        CreatedByUser = userId;
        Name = name;
        Description = description;
        Status = ProjectStatus.Active;
    }

    public void SetDueDate(DateTime date)
    {
        if (date <= DateTime.UtcNow)
            throw new ArgumentException("Due date must be in the future.");
        DueAt = date;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty or null");
        Description = description;
    }
}
