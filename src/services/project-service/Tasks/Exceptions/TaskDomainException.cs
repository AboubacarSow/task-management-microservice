namespace project_service.Tasks.Exceptions;

public class TaskDomainException(string message): Exception(message)
{}

public class TaskInvalidOperationException(string message) : InvalidOperationException(message) { }