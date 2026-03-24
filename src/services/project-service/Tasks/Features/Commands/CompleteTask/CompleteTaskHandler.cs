namespace project_service.Tasks.Features.Commands.CompleteTask;

public record CompleteTaskCommand(Guid TaskId,Guid CurrentUserId);
public class CompleteTaskHandler
{
    public CompleteTaskHandler(ITaskRepository object1, ILogger<CompleteTaskHandler> object2)
    {
    }

    public async Task Handle(CompleteTaskCommand command, object value)
    {
        throw new NotImplementedException();
    }
}