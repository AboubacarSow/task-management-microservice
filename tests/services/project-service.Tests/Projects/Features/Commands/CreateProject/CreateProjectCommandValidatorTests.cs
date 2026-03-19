using FluentValidation.TestHelper;
using project_service.Projects.Features.Commands.CreateProject;

namespace project_service.Tests.Projects.Features.Commands.CreateProject;

public class CreateProjectCommandValidatorTests
{
    [Fact]
    public void Validator_ForValidCommand_ShouldNotHaveValidationErrors()
    {
        var userId = Guid.NewGuid();
        var command = new CreateProjectCommand("Building a web crawler in .Net",userId,null);

        var validator = new CreateProjectCommandValidator();

        var result = validator.TestValidate(command);


        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_ForNonValidCommand_ShouldHaveValitionErrors()
    {
        var userId = Guid.Empty;
        var command = new CreateProjectCommand(string.Empty, userId, null);

        var validator = new CreateProjectCommandValidator();

        var result = validator.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(c=>c.Name);
        result.ShouldHaveValidationErrorFor(c=>c.CreatedByUser);
    }
}
