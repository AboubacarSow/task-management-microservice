<<<<<<< HEAD
namespace task_service.Tasks.Dtos;

public record TaskItemDto(
    Guid Id,
    string Name,
    Guid ProjectId,
    Guid CreatedByUser,
    Guid? AssignedToUser,
    TaskPriority Priority,
    string Status,
    DateTime? DueAt,
    string? Description,
    DateTime CreatedAt,
    DateTime LastUpdatedAt,
    string? Note);
=======
namespace task_service.Tasks.Dtos;

public record TaskItemDto(
    Guid Id,
    string Name,
    Guid ProjectId,
    Guid CreatedByUser,
    Guid? AssignedToUser,
    TaskPriority Priority,
    string Status,
    DateTime? DueAt,
    string? Description,
    DateTime CreatedAt,
    DateTime LastUpdatedAt,
    string? Note);
>>>>>>> 05b451b (new_update)
