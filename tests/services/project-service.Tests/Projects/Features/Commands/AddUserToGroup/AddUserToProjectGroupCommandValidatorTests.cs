using FluentValidation.TestHelper;
using project_service.Projects.Features.Commands.AddUserToGroup;
using Xunit;

namespace project_service.Tests.Projects.Features.Commands.AddUserToGroup;

public class AddUserToGroupCommandValidatorTests
{
    private readonly AddUserToGroupCommandValidator _validator;

    public AddUserToGroupCommandValidatorTests()
    {
        _validator = new AddUserToGroupCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_ProjectId_Is_Empty()
    {
        var command = new AddUserToGroupCommand(
            Guid.Empty,
            Guid.NewGuid()
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new AddUserToGroupCommand(
            Guid.NewGuid(),
            Guid.Empty
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new AddUserToGroupCommand(
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}