using Microsoft.AspNetCore.Http.HttpResults;
using project_service.Tasks.Exceptions;

namespace project_service.Tasks.Models;

public class Task
{
    public string Name{get;}
    public Guid ProjectId{get;private set;}
    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public DateTime LastUpdatedAt { get; private set; }
    public Guid? AssignedToUser { get;private set; }
    public TaskStatus Status { get; private set; }
    public string? Description{get;private set;}
    public DateTime DueAt { get;private set; }

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
        Status = TaskStatus.ToDo;
    }
    public void AssignTo(Guid userId)
    {
        if(userId.Equals(Guid.Empty))
            throw new ArgumentException("User Id cannot be null or empty");
        AssignedToUser = userId;
        Touch();
    }

    public void SetDescription(string description)
    {
        if(string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty");
        Description = description;
    }

    public void StartWork()
    {
        Status = TaskStatus.InProgress;
        Touch();
        
    }

    public void CompleteTask(string? note=null)
    {
        Status = TaskStatus.Completed;
    }

    public void UnAssign()
    {
        if(Status == TaskStatus.Completed)
            throw new TaskDomainException("Cannot unassign on completed task");
        AssignedToUser=null;
        Touch();
        if (Status == TaskStatus.InProgress)
        {
            Status = TaskStatus.Pause;
            return;
        }
        Status = TaskStatus.ToDo;
    }

    private void Touch()
    {
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void ReassignTo(Guid new_userId)
    {
        AssignedToUser = new_userId;
    }

    public void SetDueAt(DateTime date)
    {
        if(date <= DateTime.UtcNow)
            throw new ArgumentException("DueAt date must be in future.");
        DueAt = date;
    }
}
