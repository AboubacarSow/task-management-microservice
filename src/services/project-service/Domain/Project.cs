namespace project_service.Domain;

public class Project
{
    public string Name { get; }
    public string Description{ get;}
    public ProjectStatus Status { get;}

    public Project(string name, string description)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Project name can not be empty");
        Name = name;
        Description = description;
        Status = ProjectStatus.Active;
    }
}


public enum ProjectStatus
{
   Complete,
   Active,
   OnHold
}
