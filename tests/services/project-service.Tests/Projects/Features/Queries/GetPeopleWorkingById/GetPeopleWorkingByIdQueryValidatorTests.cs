using project_service.Projects.Features.Queries.GetPeopleWorkingById;

namespace project_service.Tests.Projects.Features.Queries.GetPeopleWorkingById;

public class GetPeopleWorkingByIdQueryValidatorTests
{
    private readonly GetPeopleWorkingByIdQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ProjectId_Is_Empty()
    {
        var query = new GetPeopleWorkingByIdQuery(Guid.NewGuid(),Guid.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var query = new GetPeopleWorkingByIdQuery(Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ProjectId_Is_Valid()
    {
        var query = new GetPeopleWorkingByIdQuery(Guid.NewGuid(),Guid.NewGuid());

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }
}

