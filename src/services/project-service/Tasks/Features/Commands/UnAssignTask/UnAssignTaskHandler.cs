

namespace project_service.Tasks.Features.Commands.UnAssignTask;


public class UnAssignTaskCommand(Guid TaskId,Guid CurrentUserId);
public class UnAssignTaskHandler
{
    public UnAssignTaskHandler(ITaskRepository object1, IProjectRepository object2)
    {
    }

    public async Task Handle(UnAssignTaskCommand command, CancellationToken none)
    {
        throw new NotImplementedException();
    }
}