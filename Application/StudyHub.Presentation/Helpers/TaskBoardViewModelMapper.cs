using Application.Models;
using StudyHub.Core.DTOs;

namespace Application.Helpers;

public static class TaskBoardViewModelMapper
{
    public static TaskBoardCreatePageViewModel MapTaskCreatePageViewModel(TaskBoardCreatePageDataDto data)
    {
        return new TaskBoardCreatePageViewModel
        {
            Board = data.Board,
            Subjects = data.Subjects.Select(item => new SubjectOptionViewModel
            {
                Id = item.Id,
                Name = item.Name,
                DisplayName = item.DisplayName
            }).ToList(),
            Title = data.Title,
            Description = data.Description,
            SubjectId = data.SubjectId,
            DueDate = data.DueDate,
            ResourceUrl = data.ResourceUrl,
            IsGroupTask = data.IsGroupTask
        };
    }

    public static GroupUserViewModel MapGroupUserViewModel(GroupUserDto data)
    {
        return new GroupUserViewModel
        {
            UserId = data.UserId,
            Name = data.Name,
            Role = data.Role
        };
    }
}
