using Carter;
using MediatR;

namespace project_service.Projects.Features.Queries.GetProjectById;

public class GetProjectByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{id:guid}", async (Guid id, ISender sender) =>
        {
            var query = new GetProjectByIdQuery(id);
            var result = await sender.Send(query);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);

        }).RequireAuthorization();
    }
}
