namespace project_service.Tasks.Features.Queries.GetTaskById;


public class GetTaskByIdEndpoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
       app.MapGet("/api/tasks/{id:guid}", async (Guid id, ISender sender, IUserContext userContext) =>
        {
            var current_user = userContext.GetUserId();
            var result = await sender.Send(new GetTaskByIdQuery(current_user,id));
            return Results.Ok(result);
        }).RequireAuthorization()
        .WithName("GetTaskById");
    }
}