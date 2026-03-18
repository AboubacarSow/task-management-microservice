using Task = project_service.Tasks.Models.Task;

namespace project_service.Tests.Helpers;

public static class FakeTaskData 
{
    public static List<Task> GetTasksForMultipleProjects(Guid project1,Guid project2)=>
    [
        new("Project1 - Task A",project1,Guid.NewGuid()),
        new("Project1 - Task B",project1,Guid.NewGuid()),
        new("Project1 - Task C",project1,Guid.NewGuid()),
        new("Project1 - Task D",project1,Guid.NewGuid()),

        new("Project2 - Task A",project2,Guid.NewGuid()),
        new("Project2 - Task B",project2,Guid.NewGuid()),
        new("Project2 - Task C",project2,Guid.NewGuid()),
        new("Project2 - Task D",project2,Guid.NewGuid()),
        new("Project2 - Task E",project2,Guid.NewGuid()),
        
    ];

    public static List<Task> GetTasksForMultipleUsers(Guid user1, Guid user2) =>
    [
        new("Project1 - Task A",Guid.NewGuid(),user1),
        new("Project1 - Task B",Guid.NewGuid(),user1),
        new("Project1 - Task C",Guid.NewGuid(),user1),
        new("Project1 - Task D",Guid.NewGuid(),user1),

        new("Project2 - Task A",Guid.NewGuid(),user2),
        new("Project2 - Task B",Guid.NewGuid(),user2),
        new("Project2 - Task C",Guid.NewGuid(),user2),
        new("Project2 - Task D",Guid.NewGuid(),user2),
        new("Project2 - Task E",Guid.NewGuid(),user2),
    ];
}

