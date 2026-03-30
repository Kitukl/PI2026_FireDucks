using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.DTOs.Parser;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;
using System.Text.RegularExpressions;
using Group = StudyHub.Domain.Entities.Group;
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
            var dbGroup = await _context.Groups.FirstOrDefaultAsync(x => schedule.Group.Id == x.Id || schedule.Group.Name == x.Name);
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
        var existingSchedule = await _context.Schedules
            .Include(s => s.Lessons)
            .FirstOrDefaultAsync(s => s.Id == schedule.Id);

        if (existingSchedule == null) return;

        existingSchedule.IsAutoUpdate = schedule.IsAutoUpdate;
        existingSchedule.CanHeadmanUpdate = schedule.CanHeadmanUpdate;
        existingSchedule.UpdatedAt = schedule.UpdatedAt;

        var newLessonIds = schedule.Lessons.Select(l => l.Id).ToList();

        existingSchedule.Lessons.Clear();

        foreach (var lessonId in newLessonIds)
        {
            var dbLesson = await _context.Lessons.FindAsync(lessonId);
            if (dbLesson != null)
            {
                existingSchedule.Lessons.Add(dbLesson);
            }
        }

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

    public async Task SyncParsedScheduleAsync(string groupName, ParsedScheduleResponse parsedData, CancellationToken cancellationToken)
    {
        _context.ChangeTracker.Clear();

        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == groupName, cancellationToken);
        if (group == null)
        {
            group = new Group { Id = Guid.NewGuid(), Name = groupName };
            _context.Groups.Add(group);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var schedule = await _context.Schedules
            .Include(s => s.Lessons)
            .FirstOrDefaultAsync(s => s.GroupId == group.Id, cancellationToken);

        if (schedule == null)
        {
            schedule = new Schedule
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                IsAutoUpdate = true,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Schedules.Add(schedule);
        }
        else
        {
            if (schedule.Lessons.Any())
            {
                _context.Lessons.RemoveRange(schedule.Lessons);
            }
            schedule.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var allSubjects = await _context.Subjects.ToDictionaryAsync(s => s.Name.ToLower(), cancellationToken);
        var allLecturers = await _context.Lecturers.ToDictionaryAsync(l => $"{l.Surname} {l.Name}".Trim().ToLower(), cancellationToken);
        var allSlots = await _context.LessonsSlots.ToListAsync(cancellationToken);

        foreach (var parsedLesson in parsedData.Lessons)
        {
            var subjectKey = parsedLesson.SubjectName.ToLower();
            if (!allSubjects.TryGetValue(subjectKey, out var subject))
            {
                subject = new Subject { Id = Guid.NewGuid(), Name = parsedLesson.SubjectName };
                _context.Subjects.Add(subject);
                allSubjects[subjectKey] = subject;
            }

            TimeOnly.TryParse(parsedLesson.StartTime, out var start);
            TimeOnly.TryParse(parsedLesson.EndTime, out var end);
            var slot = allSlots.FirstOrDefault(s => s.StartTime == start && s.EndTime == end);
            if (slot == null)
            {
                slot = new LessonsSlot { Id = Guid.NewGuid(), StartTime = start, EndTime = end };
                _context.LessonsSlots.Add(slot);
                allSlots.Add(slot);
            }

            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                Day = (DayOfWeek)parsedLesson.Day,
                LessonType = parsedLesson.LessonType,
                Subject = subject,
                LessonsSlot = slot,
                Schedules = new List<Schedule> { schedule },
                Lecturers = new List<Lecturer>(),
                Room = parsedLesson.Room
            };

            foreach (var t in parsedLesson.Teachers)
            {
                var lecturerKey = $"{t.Surname} {t.Name}".Trim().ToLower();
                if (!allLecturers.TryGetValue(lecturerKey, out var lecturer))
                {
                    lecturer = new Lecturer { Id = Guid.NewGuid(), Surname = t.Surname, Name = t.Name };
                    _context.Lecturers.Add(lecturer);
                    allLecturers[lecturerKey] = lecturer;
                }
                lesson.Lecturers.Add(lecturer);
            }

            _context.Lessons.Add(lesson);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _context.ChangeTracker.Clear();
    }
}