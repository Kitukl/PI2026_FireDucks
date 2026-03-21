using StudyHub.Domain.Entities;
using Schedule = StudyHub.Domain.Entities.Schedule;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Interfaces
{
    public interface IScheduleRepository
    {
        Task<Schedule?> GetByGroupIdAsync(Guid groupId);
        Task<List<Lesson>> GetLessonsByDay(Guid id, DayOfWeek Day);
        Task AddSchedule(Schedule schedule);
        Task DeleteScheduleForGroup(Guid groupId);
        Task DeleteAllAsync();
        Task UpdateScheduleAsync(Schedule schedule);
        Task UpdateHeadmanRights(Schedule schedule);
        Task<bool> GetHeadmanUpdateRights(Guid groupId);
        Task SetScheduleAutoUpdate(bool value);
        Task SetScheduleAutoUpdateInterval(uint interval);
    }
}