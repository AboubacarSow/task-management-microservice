using task_service.Tasks.Grpc.Client;

namespace task_service.Tasks.Features.Queries.GetAllByProjectId;


public record GetAllByProjectIdQuery(Guid CurrentUserId,Guid ProjectId):IRequest<List<TaskItemDto>>;

public class GetAllByProjectIdQueryValidator:AbstractValidator<GetAllByProjectIdQuery>
{
    public GetAllByProjectIdQueryValidator()
    {
        RuleFor(q=>q.ProjectId).NotEmpty().WithMessage("ProjectId is required");

        RuleFor(q=>q.CurrentUserId).NotEmpty();
    }
}
public class GetAllByProjectIdHandler(ITaskRepository _taskRepository, IProjectClient _projectClient,
 ILogger<GetAllByProjectIdHandler> _logger): IRequestHandler<GetAllByProjectIdQuery, List<TaskItemDto>>
{

    public async Task<List<TaskItemDto>> Handle(GetAllByProjectIdQuery query, CancellationToken none)
    {
        var projectModel = await _projectClient.GetProjectAsync(query.ProjectId.ToString());

         if(projectModel is null)
        {
            _logger.LogWarning("Project with Id:{ProjectId} not found.Tasks could not be loaded",query.ProjectId);
            throw new NotFoundException("Project",query.ProjectId.ToString());
        }
        var isInPeople = projectModel.PeopleWorking
            .Any(g => Guid.Parse(g) == query.CurrentUserId);
        if (!isInPeople)
        {
            _logger.LogWarning("User not authorized to perform this operation:{Operation}", "READ_TASKS");
            throw new ForbiddenException(query.CurrentUserId.ToString(), "READ_PROJECT");
        }
        
        var tasks = await _taskRepository.GetAllByProjectIdAsync(query.ProjectId);
        if (!tasks.Any())
        {
            _logger.LogWarning("No tasks for project with Id:{ProjectId}", query.ProjectId.ToString());
            return [];
        }
        _logger.LogInformation(
            "{Count} tasks for project with Id:{ProjectId}",
            tasks.Count, query.ProjectId);

        return tasks.Adapt<List<TaskItemDto>>(options =>
        {
            options.ForType<TaskItem, TaskItemDto>()
                .Map(dest => dest.Status, src => src.Status.ToString());
        });
    }
}