using StudyHub.Core.DTOs.Parser;
using StudyHub.Domain.Entities;
using Schedule = StudyHub.Domain.Entities.Schedule;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Interfaces
{
    public interface IScheduleRepository
    {
        Task<Schedule?> GetById(Guid id);
        Task<List<Schedule>> GetAll();
        Task<Schedule?> GetByGroupIdAsync(Guid groupId);
        Task<List<Lesson>> GetLessonsByDay(Guid id, DayOfWeek Day);
        Task AddSchedule(Schedule schedule);
        Task DeleteScheduleForGroup(Guid groupId);
        Task DeleteAllAsync();
        Task UpdateScheduleAsync(Schedule schedule);
        Task UpdateLeaderRights(Schedule schedule);
        Task<bool> GetLeaderUpdateRights(Guid groupId);
        Task SetScheduleAutoUpdate(bool value);
        Task SetScheduleAutoUpdateInterval(uint interval);
        Task UpdateGlobalSettings(bool isAutoUpdate, bool allowLeaders, uint intervalDays, DateTime updatedAt);
        Task SyncParsedScheduleAsync(string groupName, ParsedScheduleResponse parsedData, CancellationToken cancellationToken);
    }
}