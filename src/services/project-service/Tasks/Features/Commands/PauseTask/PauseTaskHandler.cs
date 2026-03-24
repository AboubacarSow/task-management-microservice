

namespace project_service.Tasks.Features.Commands.PauseTask;


public record PauseTaskCommand(Guid id, Guid userId, string V);
public class PauseTaskHandler
{
    public PauseTaskHandler(ITaskRepository object1, IProjectRepository object2, ILogger<PauseTaskHandler> object3)
    {
    }

    public async Task Handle(PauseTaskCommand command, CancellationToken none)
    {
        throw new NotImplementedException();
    }
}
