using Application.Helpers;
using Application.Models;
using StudyHub.Domain.Entities;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Core.Common;

public static class DashboradHelper
{ 
    public static async Task<TaskBoardPageViewModel> BuildTaskBoardModelAsync(User user, IEnumerable<Task> tasks)
    {
        if (user is null)
        {
            return new TaskBoardPageViewModel
            {
                Tasks = new List<TaskBoardTaskCardViewModel>(),
                Subjects = new List<string>()
            };
        }

        var sortedTasks = tasks
            .OrderBy(task => task.Subject.Name)
            .ThenBy(task => task.CreatedAt)
            .ThenBy(task => task.Title)
            .ToList();

        var subjectCounters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var cards = new List<TaskBoardTaskCardViewModel>(sortedTasks.Count);

        foreach (var task in sortedTasks)
        {
            var subjectName = string.IsNullOrWhiteSpace(task.Subject.Name) ? "Unknown" : task.Subject.Name;
            if (!subjectCounters.TryAdd(subjectName, 1))
            {
                subjectCounters[subjectName] += 1;
            }

            var prefix = TaskFormattingHelper.GenerateTaskCodePrefix(subjectName);
            var taskCode = $"{prefix}-{subjectCounters[subjectName]}";

            cards.Add(new TaskBoardTaskCardViewModel
            {
                Id = task.Id,
                SubjectId = task.Subject.Id,
                Title = task.Title,
                Description = task.Description,
                SubjectName = subjectName,
                TaskCode = taskCode,
                IsGroupTask = task.IsGroupTask,
                Status = task.Status,
                Deadline = task.Deadline
            });
        }

        return new TaskBoardPageViewModel
        {
            Tasks = cards,
            Subjects = cards
                .Select(card => card.SubjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(subject => subject)
                .ToList()
        };
    }
    
    public static bool IsVisibleForUser(Task task, Guid userId, string? groupName)
    {
        if (task.User.Id == userId)
        {
            return true;
        }

        if (!task.IsGroupTask || string.IsNullOrWhiteSpace(groupName))
        {
            return false;
        }

        return string.Equals(task.User.Group?.Name, groupName, StringComparison.OrdinalIgnoreCase);
    }
}