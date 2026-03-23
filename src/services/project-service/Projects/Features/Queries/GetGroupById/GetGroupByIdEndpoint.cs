using Carter;
using MediatR;
using project_service.Data.Utilities;

namespace project_service.Projects.Features.Queries.GetGroupById;

public class GetGroupByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/group",
            async (Guid projectId, ISender sender, IUserContext userContext) =>
            {
                var currentUserId= userContext.GetUserId(); 

                var result = await sender.Send(new GetGroupByIdQuery(currentUserId,projectId));


                return Results.Ok(result);
            })
            .RequireAuthorization();
    }
}