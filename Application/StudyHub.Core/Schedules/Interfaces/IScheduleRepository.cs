using StudyHub.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Schedule.Interfaces
{
    public interface IScheduleRepository
    {
        Task<ScheduleDto?> GetByGroupIdAsync(Guid groupId);
        Task<List<LessonDto>> GetLessonsByDay(Guid id, int Day);
        Task AddSchedule(ScheduleDto schedule);
        Task DeleteScheduleForGroup(Guid groupId);
        Task DeleteAllAsync();
        Task UpdateScheduleAsync(ScheduleDto schedule);
        Task UpdateHeadmanRights(ScheduleHeadmanRightsUpdateDto dto);
        Task<bool> GetHeadmanUpdateRights(Guid groupId);
        Task SetScheduleAutoUpdate();
        Task SetScheduleAutoUpdateInterval(uint interval);
    }
}