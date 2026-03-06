using Microsoft.AspNetCore.Http.HttpResults;

namespace project_service.Tasks.Models;

public class Task
{
    public string Name{get;}
    public Guid ProjectId{get;private set;}
    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime LastUpdatedAt { get; private set; }

    public Task(string name, Guid projectId)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Task name cannot be null or empty");
        if(projectId.Equals(Guid.Empty))
            throw new ArgumentException("Task cannot be created without its project specified");
        Name = name;
        Id=Guid.NewGuid();
        ProjectId = projectId;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = CreatedAt;
    }
    
}