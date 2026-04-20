<<<<<<< HEAD
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using task_grpc_server;

namespace task_service.Tasks.Grpc.Server;

[Authorize]
public class TaskGrpcService(ITaskRepository taskRepository, 
        ILogger<TaskGrpcService> logger) : TaskInfo.TaskInfoBase
{
    private readonly ITaskRepository _repository = taskRepository;
    private readonly ILogger<TaskGrpcService> _logger = logger;


    public override async Task<TaskModel> GetTaskById(GetTaskRequest request, ServerCallContext context)
    {
        var projectId = Guid.Parse(request.ProjectId);
        var areAllCompleted = await _repository.AreAllTasksCompletedForProjectIdAsync(projectId);
        
        _logger.LogInformation("Checked if all tasks for project with id {ProjectId} are completed: {AreAllCompleted}", request.ProjectId, areAllCompleted);

        var model = new TaskModel
        {
            AllCompleted = areAllCompleted
        };

        return model;
    }
=======
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using task_grpc_server;

namespace task_service.Tasks.Grpc.Server;

[Authorize]
public class TaskGrpcService(ITaskRepository taskRepository, 
        ILogger<TaskGrpcService> logger) : TaskInfo.TaskInfoBase
{
    private readonly ITaskRepository _repository = taskRepository;
    private readonly ILogger<TaskGrpcService> _logger = logger;


    public override async Task<TaskModel> GetTaskById(GetTaskRequest request, ServerCallContext context)
    {
        var projectId = Guid.Parse(request.ProjectId);
        var areAllCompleted = await _repository.AreAllTasksCompletedForProjectIdAsync(projectId);
        
        _logger.LogInformation("Checked if all tasks for project with id {ProjectId} are completed: {AreAllCompleted}", request.ProjectId, areAllCompleted);

        var model = new TaskModel
        {
            AllCompleted = areAllCompleted
        };

        return model;
    }
>>>>>>> 05b451b (new_update)
}