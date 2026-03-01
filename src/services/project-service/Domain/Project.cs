namespace project_service.Domain;

public class Project
{
    public string Name { get; }
    public string? Description { get;}
    public ProjectStatus Status { get; }

    public Project(string name, string? description =null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name can not be empty");

        if (description is not null && string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Project description can not be empty");
        Name = name;
        Description = description;
        Status = ProjectStatus.Active;
    }
}
