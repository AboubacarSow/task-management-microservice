namespace project_service.Tasks.Features.Commands.CompleteTask;
public record CompleteTaskRequest(string Notes);
public class CompleteTaskEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id:guid}/complete", async ([FromRoute]Guid id, 
        [FromBody]CompleteTaskRequest request, 
        [FromServices]ISender sender, 
        ClaimsPrincipal claims)=>
        {
            var userId = Guid.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

            var command = new CompleteTaskCommand(id,userId,request.Notes);
            await sender.Send(command);

            return Results.NoContent();
        }).RequireAuthorization()
        .WithName("CompleteTask");
    }
}