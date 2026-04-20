using project_service.Projects.Features.Commands.EditProject;

namespace project_service.Tests.Projects.Features.Commands.EditProject;

public class EditProjectCommandValidatorTests
{
    private readonly EditProjectCommandValidator _validator = new();

   
    [Fact]
    public async Task Validate_EmptyProjectId_ReturnsValidationError()
    {
        // Arrange
        var command = new EditProjectCommand(
            ProjectId:   Guid.Empty,
            UserId:      Guid.NewGuid(),
            Description: null,
            DueAt:       null);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public async Task Validate_EmptyUserId_ReturnsValidationError()
    {
        // Arrange
        var command = new EditProjectCommand(
            ProjectId:   Guid.NewGuid(),
            UserId:      Guid.Empty,
            Description: null,
            DueAt:       null);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public async Task Validate_ValidProjectIdAndUserId_ReturnsNoErrors()
    {
        // Arrange
        var command = new EditProjectCommand(
            ProjectId:   Guid.NewGuid(),
            UserId:      Guid.NewGuid(),
            Description: null,
            DueAt:       null);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_NullDescription_ReturnsNoErrors()
    {
        // Arrange
        var command = new EditProjectCommand(
            ProjectId:   Guid.NewGuid(),
            UserId:      Guid.NewGuid(),
            Description: null,
            DueAt:       null);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain(e => e.PropertyName == "Description");
    }
    [Fact]
    public async Task Validate_NullDueAt_ReturnsNoErrors()
    {
        // Arrange
        var command = new EditProjectCommand(
            ProjectId:   Guid.NewGuid(),
            UserId:      Guid.NewGuid(),
            Description: null,
            DueAt:       null);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain(e => e.PropertyName == "DueAt");
    }
}
