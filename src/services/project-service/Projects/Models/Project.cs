namespace project_service.Projects.Models;

public class Project
{

    public string Name { get; }
    public string? Description { get;}
    public ProjectStatus Status { get; }
    public Guid Id { get; }
    public  DateTime CreatedAt{get;}
    public Guid CreatedByUser { get;}
    public DateTime LastUpdatedAt { get;private set; }
    public DateTime? DueAt { get;private set; }

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
}
