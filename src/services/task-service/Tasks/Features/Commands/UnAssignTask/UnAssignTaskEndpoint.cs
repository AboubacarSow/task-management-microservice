namespace task_service.Tasks.Features.Commands.UnAssignTask;

public class UnAssignTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        
        app.MapPatch("/api/tasks/{id:guid}/unassign",
            async (Guid id, [FromServices]ISender sender, [FromServices]ClaimsPrincipal claims) =>
        {
            var userId = Guid.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
            await sender.Send(new UnAssignTaskCommand(id, userId));
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("UnassignTask");
    }

   
}