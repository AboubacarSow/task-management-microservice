using FluentValidation.TestHelper;
using project_service.Projects.Features.Commands.AddUserToPeople;

namespace project_service.Tests.Projects.Features.Commands.AddUserToPeopleWorking;

public class AddUserToPeopleWorkingCommandValidatorTests
{
    private readonly AddUserToPeopleWorkingCommandValidator _validator;

    public AddUserToPeopleWorkingCommandValidatorTests()
    {
        _validator = new AddUserToPeopleWorkingCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_ProjectId_Is_Empty()
    {
        var command = new AddUserToPeopleWorkingCommand(
            Guid.Empty,
            Guid.NewGuid()
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new AddUserToPeopleWorkingCommand(
            Guid.NewGuid(),
            Guid.Empty
        );

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TargetUserId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new AddUserToPeopleWorkingCommand(
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}