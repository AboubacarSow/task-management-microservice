
namespace project_service.Projects.Features.Commands.CreateProject;


public record CreateProjectResponse(Guid Id);
public record CreateProjectRequest(string Name,string? Description);
public class CreateProjectEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects", async ([FromBody]CreateProjectRequest request,
        [FromServices]ISender sender ,[FromServices]ClaimsPrincipal claims) => 
        {
            var userId =Guid.Parse(claims.FindFirst("sub")?.Value!); 
            var command = new CreateProjectCommand(request.Name,userId,request.Description);
            var result = await sender.Send(command);
            var response = new CreateProjectResponse(
                result);
            return Results.Created($"/api/projects/{response.Id}",response);
        }).RequireAuthorization();
    }
}