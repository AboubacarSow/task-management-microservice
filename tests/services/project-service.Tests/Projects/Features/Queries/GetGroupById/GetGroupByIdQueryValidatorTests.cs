using project_service.Projects.Features.Queries.GetGroupById;

namespace project_service.Tests.Projects.Features.Queries.GetGroupById;

public class GetGroupByIdQueryValidatorTests
{
    private readonly GetGroupByIdQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ProjectId_Is_Empty()
    {
        var query = new GetGroupByIdQuery(Guid.NewGuid(),Guid.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var query = new GetGroupByIdQuery(Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ProjectId_Is_Valid()
    {
        var query = new GetGroupByIdQuery(Guid.NewGuid(),Guid.NewGuid());

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
