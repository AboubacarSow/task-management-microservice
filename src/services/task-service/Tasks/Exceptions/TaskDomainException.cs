<<<<<<< HEAD
namespace task_service.Tasks.Exceptions;

public class TaskDomainException(string message): Exception(message)
{}


=======
namespace task_service.Tasks.Exceptions;

public class TaskDomainException(string message): Exception(message)
{}


>>>>>>> 05b451b (new_update)
public class TaskInvalidOperationException(string message) : InvalidOperationException(message) { }