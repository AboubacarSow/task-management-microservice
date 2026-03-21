using project_service.Data.Repositories;

namespace project_service.Projects.Features.Commands.EditProject;


public record EditProjectCommand
{
    public EditProjectCommand(Guid guid1, Guid guid2, string? description, DateTime? dueAt)
    {
    }
}
public class EditProjectHandler
{
    public EditProjectHandler(IProjectRepository repository, ILogger<EditProjectHandler> logger)
    {
    }

    public Task<object> Handle(EditProjectCommand command, CancellationToken none)
    {
        throw new NotImplementedException();
    }
}

