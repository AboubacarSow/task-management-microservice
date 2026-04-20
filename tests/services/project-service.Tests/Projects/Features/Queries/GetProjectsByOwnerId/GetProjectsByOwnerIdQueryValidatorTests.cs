<<<<<<< HEAD
using project_service.Projects.Features.Queries.GetProjectsByOwnerId;

namespace project_service.Tests.Projects.Features.Queries.GetProjectsByOwnerId;

public class GetProjectsByOwnerIdQueryValidatorTests
{
    private readonly GetProjectsByOwnerIdQueryValidator _validator = new();

    [Fact]
    public void Validate_Should_Pass_When_OwnerId_Is_Valid()
    {
        // Arrange
        var query = new GetProjectsByOwnerIdQuery(Guid.NewGuid());

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_When_OwnerId_Is_Empty()
    {
        // Arrange
        var query = new GetProjectsByOwnerIdQuery(Guid.Empty);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(GetProjectsByOwnerIdQuery.OwnerId));
    }
}
=======
using project_service.Projects.Features.Queries.GetProjectsByOwnerId;

namespace project_service.Tests.Projects.Features.Queries.GetProjectsByOwnerId;

public class GetProjectsByOwnerIdQueryValidatorTests
{
    private readonly GetProjectsByOwnerIdQueryValidator _validator = new();

    [Fact]
    public void Validate_Should_Pass_When_OwnerId_Is_Valid()
    {
        // Arrange
        var query = new GetProjectsByOwnerIdQuery(Guid.NewGuid());

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_When_OwnerId_Is_Empty()
    {
        // Arrange
        var query = new GetProjectsByOwnerIdQuery(Guid.Empty);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == nameof(GetProjectsByOwnerIdQuery.OwnerId));
    }
}
>>>>>>> 05b451b (new_update)
