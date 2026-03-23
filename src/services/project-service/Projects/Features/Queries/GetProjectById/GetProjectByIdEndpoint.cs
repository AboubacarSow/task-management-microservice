using Carter;
using MediatR;
using project_service.Data.Utilities;

namespace project_service.Projects.Features.Queries.GetProjectById;

public class GetProjectByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{id:guid}", 
                async (Guid id, ISender sender,IUserContext userContext) =>
        {

            //ToDo
            // Only user in PeopleWorking can see project
            var currentUserId = userContext.GetUserId();
            var query = new GetProjectByIdQuery(currentUserId,id);
            var result = await sender.Send(query);

            return Results.Ok(result);

        }).RequireAuthorization()
        .WithName("GetById");
    }
}
