using Application.Models;
using MediatR;
using StudyHub.Core.Admin.DTOs;
using StudyHub.Core.DTOs;
using StudyHub.Core.Group.Queries;
using StudyHub.Core.Schedules.Queries;
using DayOfWeek = StudyHub.Domain.Enums.DayOfWeek;

namespace StudyHub.Core.Admin.Queries;

public class GetAdminSchedulePageQuery : IRequest<AdminSchedulePageDataDto>
{
    public Guid? GroupId { get; set; }
}

public class GetAdminSchedulePageQueryHandler : IRequestHandler<GetAdminSchedulePageQuery, AdminSchedulePageDataDto>
{
    private readonly ISender _sender;

    public GetAdminSchedulePageQueryHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<AdminSchedulePageDataDto> Handle(GetAdminSchedulePageQuery request, CancellationToken cancellationToken)
    {
        var groups = (await _sender.Send(new GetAllGroupsQuery(), cancellationToken))
            .OrderBy(group => group.Name)
            .Select(group => new GroupDto { Id = group.Id, Name = group.Name })
            .ToList();

        var allSchedules = await _sender.Send(new GetAllSchedulesRequest(), cancellationToken);
        var firstWithSettings = allSchedules.FirstOrDefault();

        var response = new AdminSchedulePageDataDto
        {
            Groups = groups,
            SelectedGroupId = request.GroupId,
            IsAutoUpdateEnabled = firstWithSettings?.IsAutoUpdate ?? false,
            AllowLeadersToUpdate = firstWithSettings?.HeadmanUpdate ?? false,
            LastGlobalUpdate = allSchedules.Any() ? allSchedules.Max(schedule => schedule.UpdateAt) : DateTime.MinValue,
            AutoUpdateIntervalDays = firstWithSettings?.UpdateInterval ?? 3
        };

        if (request.GroupId.HasValue)
        {
            var scheduleDto = await _sender.Send(new GetScheduleByGroupIdRequest(request.GroupId.Value), cancellationToken);
            if (scheduleDto != null)
            {
                response.SelectedGroupLastUpdate = scheduleDto.UpdateAt;
                response.CurrentGroupSchedule = BuildScheduleGridViewModel(scheduleDto);
            }
        }

        return response;
    }

    private static ScheduleViewModel BuildScheduleGridViewModel(ScheduleDto dto)
    {
        var vm = new ScheduleViewModel
        {
            GroupId = dto.Group.Id,
            GroupName = dto.Group.Name,
            CanHeadmanUpdate = dto.HeadmanUpdate,
            IsHeadman = false
        };

        if (dto.Lessons != null && dto.Lessons.Any())
        {
            vm.UniqueSlots = dto.Lessons
                .Select(lesson => lesson.LessonSlot)
                .Where(slot => slot != null)
                .GroupBy(slot => slot.Id)
                .Select(group => group.First())
                .OrderBy(slot => slot.StartTime)
                .ToList();

            vm.Days =
            [
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday
            ];

            foreach (var lesson in dto.Lessons)
            {
                var key = $"{(int)lesson.Day}-{lesson.LessonSlot.Id}";

                if (!vm.Grid.ContainsKey(key))
                {
                    vm.Grid[key] = new List<LessonDto>();
                }

                vm.Grid[key].Add(lesson);
            }
        }

        return vm;
    }
}
