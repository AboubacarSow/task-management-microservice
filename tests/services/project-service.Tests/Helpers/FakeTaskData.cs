using TaskItem = project_service.Tasks.Models.TaskItem;

namespace project_service.Tests.Helpers;

public static class FakeTaskData 
{
    public static List<TaskItem> GetTasksForMultipleProjects(Guid project1,Guid project2)=>
    [
        new("Project1 - TaskItem A",project1,Guid.NewGuid()),
        new("Project1 - TaskItem B",project1,Guid.NewGuid()),
        new("Project1 - TaskItem C",project1,Guid.NewGuid()),
        new("Project1 - TaskItem D",project1,Guid.NewGuid()),

        new("Project2 - TaskItem A",project2,Guid.NewGuid()),
        new("Project2 - TaskItem B",project2,Guid.NewGuid()),
        new("Project2 - TaskItem C",project2,Guid.NewGuid()),
        new("Project2 - TaskItem D",project2,Guid.NewGuid()),
        new("Project2 - TaskItem E",project2,Guid.NewGuid()),
        
    ];

    public static List<TaskItem> GetTasksForMultipleUsers(Guid user1, Guid user2) =>
    [
        new("Project1 - TaskItem A",Guid.NewGuid(),user1),
        new("Project1 - TaskItem B",Guid.NewGuid(),user1),
        new("Project1 - TaskItem C",Guid.NewGuid(),user1),
        new("Project1 - TaskItem D",Guid.NewGuid(),user1),

        new("Project2 - TaskItem A",Guid.NewGuid(),user2),
        new("Project2 - TaskItem B",Guid.NewGuid(),user2),
        new("Project2 - TaskItem C",Guid.NewGuid(),user2),
        new("Project2 - TaskItem D",Guid.NewGuid(),user2),
        new("Project2 - TaskItem E",Guid.NewGuid(),user2),
    ];
}

