namespace project_service.Domain;

public class Project
{
    public string Name{get;}

    public Project(string name)
    {
        if(string.IsNullOrEmpty(name))
            throw new ArgumentException("Project name can not be empty");
        Name = name;
    }
}