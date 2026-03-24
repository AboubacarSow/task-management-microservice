

namespace project_service.Tasks.Dtos;

public record TaskItemDto(
    string Name,
    Guid ProjectId,
    Guid CreatedByUser,
    Guid? AssignedToUser,
    TaskPriority Priority,
    Models.TaskStatus Status,
    DateTime? DueAt,
    string? Description,
    DateTime CreatedAt,
    DateTime LastUpdatedAt,
    string? Note);
