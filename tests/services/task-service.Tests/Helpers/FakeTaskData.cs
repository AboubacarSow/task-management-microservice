<<<<<<< HEAD
using task_service.Tasks.Models;

namespace task_service.Tests.Helpers;

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

=======
using MassTransit.NewIdProviders;
using project_grpc_server;
using task_service.Tasks.Models;

namespace task_service.Tests.Helpers;

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

public static class FakeProjectModelData 
{
    public static ProjectModel BuildProjectModel(
      Guid projectId,
      Guid ownerId,
      IEnumerable<Guid>? group = null,
      IEnumerable<Guid>? peopleWorking = null)
    {
        var model = new ProjectModel
        {
            Id = projectId.ToString(),
            Name = "Building Microservice",
            OwnerId = ownerId.ToString(),
        };

        if (group != null)
            model.Group.AddRange(group.Select(g => g.ToString()));
        else
            model.Group.Add(Guid.NewGuid().ToString());

        if (peopleWorking != null)
            model.PeopleWorking.AddRange(peopleWorking.Select(g => g.ToString()));
        else
            model.PeopleWorking.AddRange(new[]
            {
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        });

        return model;
    }
}



>>>>>>> 05b451b (new_update)
