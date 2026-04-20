using MassTransit;
using project_service.Projects.Features.Commands.AddUserToPeople;
using shared.messaging.Events;

namespace project_service.Projects.EventHandlers;

public class TaskAssignedIntegrationEventHandler(ISender sender,
    ILogger<TaskAssignedIntegrationEventHandler> logger)
  : IConsumer<TaskAssignedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<TaskAssignedIntegrationEvent> context)
    {
        logger.LogInformation("Integration Event Handling:{integration}",context.Message.GetType().Name);
        var evt = context.Message;

        logger.LogInformation(
            "Received TaskAssignedEvent. TaskId: {TaskId}, UserId: {UserId}, ProjectId: {ProjectId}",
            evt.TaskId, evt.AssignedUserId, evt.ProjectId);

        await sender.Send(new AddUserToPeopleWorkingCommand
            (evt.ProjectId, evt.AssignedUserId));
        
    }
}