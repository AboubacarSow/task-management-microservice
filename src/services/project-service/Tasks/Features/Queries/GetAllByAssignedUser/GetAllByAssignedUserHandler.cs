

using project_service.Tasks.Dtos;

namespace project_service.Tasks.Features.Queries.GetAllByAssignedUser;

public record GetAllByAssignedUserQuery(Guid CurrentUserId);

public class GetAllByAssignedUserHandler
{
    public GetAllByAssignedUserHandler(ITaskRepository object1, ILogger<GetAllByAssignedUserQuery> object2)
    {
    }

    public async Task<List<TaskItemDto>> Handle(GetAllByAssignedUserQuery query, CancellationToken none)
    {
        throw new NotImplementedException();
    }
}
