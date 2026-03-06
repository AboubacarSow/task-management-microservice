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
    public string? Note { get; private set; }
    public TaskPriority Priority { get; private set; }
    public Guid CreatedByUser { get; }

    public Task(string name, Guid projectId, Guid userId)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Task name cannot be null or empty");
        if(projectId.Equals(Guid.Empty))
            throw new ArgumentException("Task cannot be created without its project specified");
        if(userId.Equals(Guid.Empty))
            throw new ArgumentException("Task cannot be created without its user author specified");
        Name = name;
        Id=Guid.NewGuid();
        ProjectId = projectId;
        CreatedByUser = userId;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = CreatedAt;
        Status = TaskStatus.ToDo;
        Priority = TaskPriority.Medium;
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
        if (AssignedToUser == null || AssignedToUser.Equals(Guid.Empty))
            throw new TaskInvalidOperationException("Task must be assigned first to be Started");
        Status = TaskStatus.InProgress;
        Touch();
        
    }

    public void CompleteTask(string? note=null)
    {
        if (Status != TaskStatus.InProgress)
            throw new TaskInvalidOperationException("Cannot mark as completed a task not in Progress");
        if (note is not null)
            Note = note;
        Status = TaskStatus.Completed;
        Touch();
    }

    public void UnAssign()
    {
        if(Status == TaskStatus.Completed)
            throw new TaskInvalidOperationException("Cannot unassign on completed task");
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
        Touch();
    }

    public void SetDueAt(DateTime date)
    {
        if(date <= DateTime.UtcNow)
            throw new ArgumentException("DueAt date must be in future.");
        DueAt = date;
    }

    public void Cancel()
    {
        if (Status == TaskStatus.Completed)
            throw new TaskInvalidOperationException("Cannot cancel a completed Task");
        Status = TaskStatus.Cancelled;
        Touch();
    }

    public void Block(string note)
    {
        if (Status != TaskStatus.InProgress)
            throw new TaskInvalidOperationException("Cannot perform this operation.Task is not in progress");
        Status = TaskStatus.Pause;
        Touch();
    }
}
