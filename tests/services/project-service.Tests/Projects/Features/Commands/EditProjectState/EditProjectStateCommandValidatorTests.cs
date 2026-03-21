using FluentValidation.TestHelper;
using project_service.Projects.Features.Commands.EditProjectState;
using project_service.Projects.Models;

namespace project_service.Tests.Projects.Features.Commands.EditProjectState;

public class EditProjectStateCommandValidatorTests
{
    private readonly EditProjectStateCommandValidator _validator;

    public EditProjectStateCommandValidatorTests()
    {
        _validator = new EditProjectStateCommandValidator();
    }

    private EditProjectStateCommand BuildValidCommand() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ProjectStatus.Active
        );


    [Fact]
    public void Should_HaveError_When_ProjectId_IsEmpty()
    {
        var command = BuildValidCommand() with { ProjectId = Guid.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }


    [Fact]
    public void Should_HaveError_When_UserId_IsEmpty()
    {
        var command = BuildValidCommand() with { UserId = Guid.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_HaveError_When_Status_IsInvalid()
    {
        var command = BuildValidCommand() with
        {
            Status = (ProjectStatus)999
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_NotHaveError_When_Command_IsValid()
    {
        var command = BuildValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}