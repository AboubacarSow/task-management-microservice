using project_service.Data.Repositories;

namespace project_service.Projects.Features.Commands.AddUserToPeople;



public record AddUserToPeopleWorkingCommand
{
    public AddUserToPeopleWorkingCommand(Guid projectId, Guid userId)
    {
    }
}
public class AddUserToPeopleWorkingHandler(IProjectRepository repository)
{
    
    public async Task Handle(AddUserToPeopleWorkingCommand command, CancellationToken none)
    {
        throw new NotImplementedException();
    }
}