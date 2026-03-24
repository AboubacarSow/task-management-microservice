using project_service.Commons;
using project_service.Tasks.Exceptions;

namespace project_service.Tasks.Models;

public sealed class TaskItem :BaseEntity
{
    public Guid ProjectId{get;private set;}
    public Guid? AssignedToUser { get;private set; }
    public TaskStatus Status { get; private set; }
    public string? Note { get; private set; }
    public TaskPriority Priority { get; private set; }
    public Guid CreatedByUser { get; private set; }

    public TaskItem(string name, Guid projectId, Guid userId)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("TaskItem name cannot be null or empty");
        if(projectId.Equals(Guid.Empty))
            throw new ArgumentException("TaskItem cannot be created without its project specified");
        if(userId.Equals(Guid.Empty))
            throw new ArgumentException("TaskItem cannot be created without its user author specified");
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
            throw new TaskInvalidOperationException("TaskItem must be assigned first to be Started");
        if(Status == TaskStatus.Completed)
            throw new TaskInvalidOperationException("*completed*");
        if (Status == TaskStatus.InProgress)
            throw new TaskInvalidOperationException("*already started*");
        Status = TaskStatus.InProgress;
        Touch();
        
    }

    public void CompleteTask(string? note=null)
    {
        if (Status != TaskStatus.InProgress)
            throw new TaskInvalidOperationException("*not in progress*");
        if (note is not null)
            Note = note;
        Status = TaskStatus.Completed;
        Touch();
    }

    public void UnAssign()
    {
        if(AssignedToUser == null)
            throw new TaskInvalidOperationException("*not assigned*");
        if(Status == TaskStatus.Completed)
            throw new TaskInvalidOperationException("Cannot unassign on completed task");
        AssignedToUser=null;
        if (Status == TaskStatus.InProgress)
        {
            Status = TaskStatus.Pause;
            return;
        }
        Status = TaskStatus.ToDo;
        Touch();
    }

    private void Touch()
    {
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void ReassignTo(Guid new_userId)
    {
        if (new_userId.Equals(Guid.Empty))
            throw new ArgumentException("User Id cannot be null or empty");
        AssignedToUser = new_userId;
        Touch();
    }

    public void SetDueAt(DateTime date)
    {
        if(date <= DateTime.UtcNow)
            throw new ArgumentException("DueAt date must be in future.");
        DueAt = date;
        Touch();
    }

    public void Cancel()
    {
        if (Status == TaskStatus.Completed)
            throw new TaskInvalidOperationException("Cannot cancel a completed TaskItem");
        Status = TaskStatus.Cancelled;
        Touch();
    }

    public void Block(string note)
    {
        if (Status != TaskStatus.InProgress)
            throw new TaskInvalidOperationException("Cannot perform this operation.TaskItem is not in progress");
        if(string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("While Blocking task, note message cannot be null or empty");
        Status = TaskStatus.Pause;
        Touch();
    }
}
