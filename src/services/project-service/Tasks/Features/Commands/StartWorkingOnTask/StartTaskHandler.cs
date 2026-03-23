

namespace project_service.Tasks.Features.Commands.StartWorkingOnTask;


public record StartTaskCommand(Guid TaskId,Guid UserId);
public class StartTaskHandler
{
    public StartTaskHandler(ITaskRepository object1, ILogger<StartTaskHandler> object2)
    {
        Object1 = object1;
        Object2 = object2;
    }

    public ITaskRepository Object1 { get; }
    public ILogger<StartTaskHandler> Object2 { get; }

    public async Task Handle(StartTaskCommand command, CancellationToken none)
    {
        throw new NotImplementedException();
    }
}