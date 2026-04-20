using task_service.Tasks.Features.Commands.CreateTaskItem;

namespace task_service.Tests.Tasks.Features.Commands.CreateTaskItem;

public class CreateTaskItemCommandValidatorTests
{
    private readonly CreateTaskItemCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ProjectId_Is_Empty()
    {
        var command = new CreateTaskItemCommand(
            Guid.NewGuid(),
            Guid.Empty,
            "Valid Title");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new CreateTaskItemCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Null()
    {
        var command = new CreateTaskItemCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new CreateTaskItemCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Build API");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}