using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;
using System.Text.RegularExpressions;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly SDbContext _context;

    public ScheduleRepository(SDbContext context)
    {
        _context = context;
    }

    public async Task<Schedule?> GetById(Guid id)
    {
        return await _context.Schedules
            .Include(s => s.Group)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Subject)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Lecturers)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.LessonsSlot)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    public async Task<List<Schedule>> GetAll()
    {
        return await _context.Schedules
            .Include(s => s.Group)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Subject)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Lecturers)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.LessonsSlot)
            .ToListAsync();
    }

    public async Task<Schedule?> GetByGroupIdAsync(Guid groupId)
    {
        return await _context.Schedules
            .Include(s => s.Group)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Subject)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Lecturers)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.LessonsSlot)
            .FirstOrDefaultAsync(s => s.Group.Id == groupId);
    }

    public async Task<List<Lesson>> GetLessonsByDay(Guid scheduleId, DayOfWeek day)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Subject)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.Lecturers)
            .Include(s => s.Lessons)
                .ThenInclude(l => l.LessonsSlot)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        return schedule?.Lessons.Where(l => l.Day == day).ToList() ?? new List<Lesson>();
    }

    public async Task AddSchedule(Schedule schedule)
    {
        if (schedule.Group != null)
        {
            var dbGroup = await _context.Groups.FindAsync(schedule.Group.Id);
            if (dbGroup != null) schedule.Group = dbGroup;
        }

        if (schedule.Lessons != null && schedule.Lessons.Any())
        {
            var processedLessons = new List<Lesson>();
            foreach (var lesson in schedule.Lessons)
            {
                var dbLesson = await _context.Lessons.FindAsync(lesson.Id);
                if (dbLesson != null)
                {
                    processedLessons.Add(dbLesson);
                }
                else
                {
                    if (lesson.Subject != null)
                    {
                        var dbSubj = await _context.Subjects.FindAsync(lesson.Subject.Id);
                        if (dbSubj != null) lesson.Subject = dbSubj;
                    }

                    if (lesson.LessonsSlot != null)
                    {
                        var dbSlot = await _context.LessonsSlots.FindAsync(lesson.LessonsSlot.Id);
                        if (dbSlot != null) lesson.LessonsSlot = dbSlot;
                    }

                    if (lesson.Lecturers != null)
                    {
                        var processedLecturers = new List<Lecturer>();
                        foreach (var lect in lesson.Lecturers)
                        {
                            var dbLect = await _context.Lecturers.FindAsync(lect.Id);
                            processedLecturers.Add(dbLect ?? lect);
                        }
                        lesson.Lecturers = processedLecturers;
                    }
                    processedLessons.Add(lesson);
                }
            }
            schedule.Lessons = processedLessons;
        }

        await _context.Schedules.AddAsync(schedule);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateScheduleAsync(Schedule schedule)
    {
        // 1. Завантажуємо існуючий розклад з бази
        var existingSchedule = await _context.Schedules
            .Include(s => s.Lessons)
            .FirstOrDefaultAsync(s => s.Id == schedule.Id);

        if (existingSchedule == null) return;

        // 2. Оновлюємо базові поля
        existingSchedule.IsAutoUpdate = schedule.IsAutoUpdate;
        existingSchedule.CanHeadmanUpdate = schedule.CanHeadmanUpdate;
        existingSchedule.UpdatedAt = schedule.UpdatedAt;

        // 3. ЗБЕРІГАЄМО ID УРОКІВ ОКРЕМО (це захист від пастки посилань)
        var newLessonIds = schedule.Lessons.Select(l => l.Id).ToList();

        // 4. Очищаємо старі зв'язки в базі
        existingSchedule.Lessons.Clear();

        // 5. Додаємо нові уроки за збереженими ID
        foreach (var lessonId in newLessonIds)
        {
            var dbLesson = await _context.Lessons.FindAsync(lessonId);
            if (dbLesson != null)
            {
                existingSchedule.Lessons.Add(dbLesson);
            }
        }

        // 6. Зберігаємо зміни
        await _context.SaveChangesAsync();
    }

    public async Task DeleteScheduleForGroup(Guid groupId)
    {
        var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Group.Id == groupId);
        if (schedule != null)
        {
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync()
    {
        _context.Schedules.RemoveRange(_context.Schedules);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateHeadmanRights(Schedule schedule)
    {
        var dbSchedule = await _context.Schedules.FindAsync(schedule.Id);
        if (dbSchedule != null)
        {
            dbSchedule.CanHeadmanUpdate = schedule.CanHeadmanUpdate;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> GetHeadmanUpdateRights(Guid groupId)
    {
        var schedule = await _context.Schedules
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Group.Id == groupId);
        return schedule?.CanHeadmanUpdate ?? false;
    }

    public async Task SetScheduleAutoUpdate(bool value)
    {
        await _context.Schedules.ExecuteUpdateAsync(s => s.SetProperty(b => b.IsAutoUpdate, value));
        await _context.SaveChangesAsync();
    }

    public async Task SetScheduleAutoUpdateInterval(uint interval)
    {
        await _context.Schedules.ExecuteUpdateAsync(s => s.SetProperty(b => b.UpdateInterval, interval));
        await _context.SaveChangesAsync();
    }
}