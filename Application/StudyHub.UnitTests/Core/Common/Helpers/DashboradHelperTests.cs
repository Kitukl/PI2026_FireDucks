using Application.Helpers;
using StudyHub.Core.Common;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using Task = System.Threading.Tasks.Task;
using TaskEntity = StudyHub.Domain.Entities.Task;

namespace StudyHub.UnitTests.Handlers.Common.Helpers;

public class DashboradHelperTests
{
    [Fact]
    public async Task BuildTaskBoardModelAsync_ShouldReturnEmptyModel_WhenUserIsNull()
    {
        // Arrange
        User? user = null;
        var tasks = new List<TaskEntity>();

        // Act
        var result = await DashboradHelper.BuildTaskBoardModelAsync(user!, tasks);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Tasks);
        Assert.Empty(result.Subjects);
    }

    [Fact]
    public async Task BuildTaskBoardModelAsync_ShouldGenerateTaskCodes_WhenTasksProvided()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var subject = new Subject { Id = Guid.NewGuid(), Name = "Computer Science" };

        var tasks = new List<TaskEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Task A",
                Description = "Description",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                Deadline = DateTime.UtcNow.AddDays(1),
                Status = Status.ToDo,
                IsGroupTask = false,
                Subject = subject,
                User = user
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Task B",
                Description = "Description",
                CreatedAt = DateTime.UtcNow,
                Deadline = DateTime.UtcNow.AddDays(2),
                Status = Status.Done,
                IsGroupTask = true,
                Subject = subject,
                User = user
            }
        };

        // Act
        var result = await DashboradHelper.BuildTaskBoardModelAsync(user, tasks);

        // Assert
        Assert.Equal(2, result.Tasks.Count);
        Assert.All(result.Tasks, card => Assert.StartsWith("COSC-", card.TaskCode));
        Assert.Single(result.Subjects);
        Assert.Equal("Computer Science", result.Subjects[0]);
    }

    [Fact]
    public void IsVisibleForUser_ShouldReturnTrue_WhenTaskOwnerMatchesUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var task = new TaskEntity
        {
            User = new User { Id = userId, Group = new Group { Name = "PI-24" } },
            Subject = new Subject { Name = "Math" }
        };

        // Act
        var result = DashboradHelper.IsVisibleForUser(task, userId, "PI-24");

        // Assert
        Assert.True(result);
    }
}
