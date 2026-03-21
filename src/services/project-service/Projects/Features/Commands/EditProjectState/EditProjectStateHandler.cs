using project_service.Data.Repositories;

namespace project_service.Projects.Features.Commands.EditProjectState;


public record EditProjectStateCommand
{
    public EditProjectStateCommand(Guid guid1, Guid guid2, object value)
    {
    }
}
public class EditProjectStateHandler
{
    public EditProjectStateHandler(IProjectRepository object1, ITaskRepository object2, ILogger<EditProjectStateHandler> object3)
    {
    }

    public Task<object> Handle(EditProjectStateCommand command, CancellationToken none)
    {
        throw new NotImplementedException();
    }
}