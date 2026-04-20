<<<<<<< HEAD
using project_service.Projects.Dtos;
using project_service.Projects.Models;

namespace project_service.Tests.Helpers;

public static class FakeProjectData
{

    public static List<Project> GetProjectsForMultipleUsers(
        Guid user1,
        Guid user2) =>
        [
            new ("User1 - Project A", user1),
            new ("User1 - Project B", user1),
            new ("User1 - Project C", user1),

            new ("User2 - Project A", user2),
            new ("User2 - Project B", user2)
        ];
   public static List<ProjectDto> GetProjectsDto(Guid ownerId)
    {
        return 
        [
            new (Guid.NewGuid(),"User Owner1",DateTime.UtcNow,
                    DateTime.UtcNow,DateTime.UtcNow.AddDays(2),
                    "details",ProjectStatus.Active,ownerId),
            new (Guid.NewGuid(),"User Owner1",DateTime.UtcNow,
                    DateTime.UtcNow,DateTime.UtcNow.AddDays(2),
                    "details",ProjectStatus.Active,ownerId),
            new (Guid.NewGuid(),"User Owner1",DateTime.UtcNow,
                    DateTime.UtcNow,DateTime.UtcNow.AddDays(2),
                    "details",ProjectStatus.Active,ownerId),
            new (Guid.NewGuid(),"User Owner1",DateTime.UtcNow,
                    DateTime.UtcNow,DateTime.UtcNow.AddDays(2),
                    "details",ProjectStatus.Active,ownerId),
        ];
    }

    public static Project BuildProject(Guid ownerId)
    => new ("Test project", ownerId,"Initial description");

    
}

=======
using project_service.Projects.Dtos;
using project_service.Projects.Models;

namespace project_service.Tests.Helpers;

public static class FakeProjectData
{

    public static List<Project> GetProjectsForMultipleUsers(
        Guid user1,
        Guid user2) =>
        [
            new ("User1 - Project A", user1),
            new ("User1 - Project B", user1),
            new ("User1 - Project C", user1),

            new ("User2 - Project A", user2),
            new ("User2 - Project B", user2)
        ];
    public static List<ProjectDto> GetProjectsDto(Guid ownerId)
    {
        return
        [
            new(
            Guid.NewGuid(),
            "User Owner1",
            DateTime.UtcNow,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(2),
            "details",
            ProjectStatus.Active.ToString(),
            ownerId,
            new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },           // Group
            new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }            // PeopleWorking
        ),
        new(
            Guid.NewGuid(),
            "User Owner1",
            DateTime.UtcNow,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(2),
            "details",
            ProjectStatus.Active.ToString(),
            ownerId,
            new List<Guid> { Guid.NewGuid() },
            new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
        ),
        new(
            Guid.NewGuid(),
            "User Owner1",
            DateTime.UtcNow,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(2),
            "details",
            ProjectStatus.Active.ToString(),
            ownerId,
            new List<Guid>(),
            new List<Guid>()
        ),
        new(
            Guid.NewGuid(),
            "User Owner1",
            DateTime.UtcNow,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(2),
            "details",
            ProjectStatus.Active.ToString(),
            ownerId,
            new List<Guid> { Guid.NewGuid() },
            new List<Guid>()
        )
        ];
    }

    public static Project BuildProject(Guid ownerId)
    => new ("Test project", ownerId,"Initial description");

    
}

>>>>>>> 05b451b (new_update)
