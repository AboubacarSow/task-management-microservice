
using project_service.Tasks.Dtos;

namespace project_service.Tasks.Features.Queries.GetAllByProjectId;


public record GetAllByProjectIdQuery(Guid CurrentUserId,Guid ProjectId);
public class GetAllByProjectIdHandler
{
    public GetAllByProjectIdHandler(ITaskRepository object1, ILogger<GetAllByProjectIdHandler> object2)
    {
    }

    public GetAllByProjectIdHandler(ITaskRepository object1, IProjectRepository object2, ILogger<GetAllByProjectIdHandler> object3)
    {
    }

    public async Task<List<TaskItemDto>> Handle(GetAllByProjectIdQuery query, CancellationToken none)
    {
        throw new NotImplementedException();
    }
}