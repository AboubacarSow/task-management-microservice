<<<<<<< HEAD
namespace task_service.Tasks.Features.Queries.GetTaskById;


public class GetTaskByIdEndpoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
       app.MapGet("/api/tasks/{id:guid}", async (Guid id, [FromServices]ISender sender, 
       ClaimsPrincipal claims) =>
        {
            var currentUserId =claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userId))
                return Results.BadRequest("Invalid user id");


            var result = await sender.Send(new GetTaskByIdQuery(userId,id));
            return Results.Ok(result);
        }).RequireAuthorization()
        .WithName("GetTaskById");
    }
=======
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
>>>>>>> 05b451b (new_update)
}