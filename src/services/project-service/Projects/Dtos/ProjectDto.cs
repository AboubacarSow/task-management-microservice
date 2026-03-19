using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using project_service.Projects.Models;

namespace project_service.Projects.Dtos;

public record ProjectDto(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime LastUpdatedAt,
    DateTime? DueAt,
    string? Description,
    ProjectStatus Status,
    Guid CreatedByUser);
