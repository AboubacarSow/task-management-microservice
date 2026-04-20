<<<<<<< HEAD
namespace project_service.Commons.Exceptions;

public class ForbiddenException(string userIdentifier,string resourceOperation)
    : Exception($"User :{userIdentifier} is not authorize to perform the [{resourceOperation}] operation")
{
=======
namespace project_service.Commons.Exceptions;

public class ForbiddenException(string userIdentifier,string resourceOperation)
    : Exception($"User :{userIdentifier} is not authorize to perform the [{resourceOperation}] operation")
{
>>>>>>> 05b451b (new_update)
}