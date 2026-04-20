using shared.Utilities;

namespace task_service.Tasks.Features.Queries.GetTaskById;


public class GetTaskByIdEndpoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
       app.MapGet("/api/tasks/{id:guid}", async (Guid id, [FromServices]ISender sender,
          [FromServices] IUserContext claims) =>
       {
           var userId = claims.GetUserId();


           var result = await sender.Send(new GetTaskByIdQuery(userId,id));
            return Results.Ok(result);
        }).RequireAuthorization()
        .WithName("GetTaskById");
    }
}