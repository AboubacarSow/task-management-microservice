namespace project_service.Commons.Exceptions;

public class ForbiddenException(string userIdentifier,string resourceOperation)
    : Exception($"User :{userIdentifier} is not authorize to perform the [{resourceOperation}] operation")
{
}