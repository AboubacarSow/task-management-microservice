namespace project_service.Domain;

public class Project
{

    public string Name { get; }
    public string? Description { get;}
    public ProjectStatus Status { get; }
    public Guid Id { get; }
    public  DateTime CreatedAt{get;}
    public Guid CreatedBy { get;}
    public DateTime LastUpdatedAt { get;private set; }
    public DateTime? DueAt { get;private set; }

    public Project(string name,Guid userId, string? description =null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name can not be empty");

        if (description is not null && string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Project description can not be empty");

        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = CreatedAt;
        CreatedBy = userId;
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
