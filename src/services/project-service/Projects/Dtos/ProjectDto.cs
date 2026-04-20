namespace project_service.Projects.Dtos;

public record ProjectDto(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime LastUpdatedAt,
    DateTime? DueAt,
    string? Description,
    string Status,
    Guid OwnerId,
    IReadOnlyCollection<Guid> Group,
    IReadOnlyCollection<Guid> PeopleWorking);
