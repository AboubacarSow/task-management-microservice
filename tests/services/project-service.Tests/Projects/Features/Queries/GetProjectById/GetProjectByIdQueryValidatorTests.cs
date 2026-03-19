using FluentAssertions;
using project_service.Projects.Features.Queries.GetProjectById;

namespace project_service.Tests.Projects.Features.Queries.GetProjectById;

public class GetProjectByIdQueryValidatorTests
{
    private readonly GetProjectByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_Should_Pass_When_ProjectId_Is_Valid()
    {
        // Arrange
        var query = new GetProjectByIdQuery(Guid.NewGuid());

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_When_ProjectId_Is_Empty()
    {
        // Arrange
        var query = new GetProjectByIdQuery(Guid.Empty);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => 
            e.PropertyName == nameof(GetProjectByIdQuery.Id));
    }
}