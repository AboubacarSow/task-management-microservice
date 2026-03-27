namespace project_service.Tasks.Features.Commands.CancelTask;


public class CancelTaskEnpoint :ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id:guid}/cancel", async ([FromRoute]Guid id,
            [FromServices]ISender sender , [FromServices]ClaimsPrincipal claims)=>
           {
                var userId = Guid.Parse(claims.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
            var command = new CancelTaskCommand(userId,id);
            await sender.Send(command);

            return Results.NoContent();
        }).RequireAuthorization()
        .WithName("CancelTask");
    }
}