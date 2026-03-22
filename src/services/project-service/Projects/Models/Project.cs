using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using project_service.Commons;

namespace project_service.Projects.Models;

public sealed class Project :BaseEntity
{

    public ProjectStatus Status { get; private set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid OwnerId { get;private set;}

    private readonly List<Guid> _group = [];
    public IReadOnlyCollection<Guid> Group => _group;

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
        OwnerId = userId;
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

    public void Complete()
    {
        if(Status == ProjectStatus.Completed)
            throw new InvalidOperationException("*already completed*");
        if(Status == ProjectStatus.OnHold)
            throw new InvalidOperationException("*OnHold project cannot be marked as Completed");
        if(Status == ProjectStatus.Archived)
            throw new InvalidOperationException("*archived");

        Status= ProjectStatus.Completed;
        
    }

    public void PutOnHold()
    {
        if(Status == ProjectStatus.OnHold)
            throw new InvalidOperationException("*already on hold*");

        if(Status == ProjectStatus.Completed)
            throw new InvalidOperationException("*completed*");

        if (Status == ProjectStatus.Archived)
            throw new InvalidOperationException("*archived*");

        Status= ProjectStatus.OnHold;
    }

    public void Reactivate()
    {
        if(Status == ProjectStatus.Active)
            throw new InvalidOperationException("*already active*");
        if(Status == ProjectStatus.Completed)
            throw new InvalidOperationException("*completed*");

        if (Status == ProjectStatus.Archived)
            throw new InvalidOperationException("*archived*");

        Status = ProjectStatus.Active;
        
    }

    public void Archive()
    {
        if (Status == ProjectStatus.Archived)
            throw new InvalidOperationException("*already archived*");
        Status = ProjectStatus.Archived;
    }

    public void AddUserToGroup(Guid userId)
    {
        if (userId == OwnerId)
            return;

        if (_group.Contains(userId))
            return;

        _group.Add(userId);
    }

    public bool IsInGroup(Guid userId)
    {
        if (userId == OwnerId)
            return true;

        return _group.Contains(userId);
    }
}
