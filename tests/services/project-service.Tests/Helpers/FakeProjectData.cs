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
}
