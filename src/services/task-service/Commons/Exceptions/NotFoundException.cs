<<<<<<< HEAD
namespace task_service.Commons.Exceptions;

public class NotFoundException(string resourceType, string resourceIdentifier)
    : Exception($"{resourceType} with id: {resourceIdentifier} not found")
{
}
=======
namespace task_service.Commons.Exceptions;

public class NotFoundException(string resourceType, string resourceIdentifier)
    : Exception($"{resourceType} with id: {resourceIdentifier} not found")
{
}
>>>>>>> 05b451b (new_update)
