using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedule.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly SDbContext _context;

    public ScheduleRepository(SDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleDto?> GetByGroupIdAsync(Guid groupId)
    {
        return await _context.Schedules
            .AsNoTracking()
            .Where(s => s.Group.Id == groupId)
            .Select(s => new ScheduleDto
            {
                Id = s.Id,
                UpdateAt = s.UpdatedAt,
                HeadmanUpdate = s.CanHeadmanUpdate,
                IsAutoUpdate = s.IsAutoUpdate,
                Group = s.Group != null ? new GroupDto
                {
                    Id = s.Group.Id,
                    Name = s.Group.Name
                } : null,
                Lessons = s.Lessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    Day = l.Day,
                    LessonType = l.LessonType,
                    Subject = new SubjectDto
                    {
                        Id = l.Subject.Id,
                        Name = l.Subject.Name
                    },
                    LessonSlot = new LessonSlotDto
                    {
                        Id = l.LessonsSlot.Id,
                        StartTime = l.LessonsSlot.StartTime,
                        EndTime = l.LessonsSlot.EndTime
                    },
                    Lecturers = l.Lecturers.Select(lec => new LecturerDto
                    {
                        Id = lec.Id,
                        Name = lec.Name,
                        Surname = lec.Surname
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<LessonDto>> GetLessonsByDay(Guid id, int day)
    {
        var targetDay = (DayOfWeek)day;

        return await _context.Lessons
            .AsNoTracking()
            .Where(l => l.Schedules.Any(s => s.Id == id) && l.Day == targetDay)
            .Select(l => new LessonDto
            {
                Id = l.Id,
                Day = l.Day,
                LessonType = l.LessonType,
                Subject = new SubjectDto { Id = l.Subject.Id, Name = l.Subject.Name },
                LessonSlot = new LessonSlotDto { Id = l.LessonsSlot.Id, StartTime = l.LessonsSlot.StartTime, EndTime = l.LessonsSlot.EndTime },
                Lecturers = l.Lecturers.Select(x => new LecturerDto { Id = x.Id, Name = x.Name, Surname = x.Surname }).ToList()
            })
            .ToListAsync();
    }

    public async Task AddSchedule(ScheduleDto scheduleDto)
    {
        var schedule = new Schedule
        {
            Id = scheduleDto.Id == Guid.Empty ? Guid.NewGuid() : scheduleDto.Id,
            UpdatedAt = scheduleDto.UpdateAt,
            CanHeadmanUpdate = scheduleDto.HeadmanUpdate,
            IsAutoUpdate = scheduleDto.IsAutoUpdate,
            Group = new Group { Id = scheduleDto.Group.Id, Name = scheduleDto.Group.Name},
            Lessons = scheduleDto.Lessons?.Select(x => new Lesson
            {
                Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
                Day = x.Day,
                LessonType = x.LessonType,
                Subject = new Subject
                {
                    Id = x.Subject?.Id == Guid.Empty ? Guid.NewGuid() : (x.Subject?.Id ?? Guid.NewGuid()),
                    Name = x.Subject?.Name ?? string.Empty
                },
                LessonsSlot = new LessonsSlot
                {
                    Id = x.LessonSlot?.Id == Guid.Empty ? Guid.NewGuid() : (x.LessonSlot?.Id ?? Guid.NewGuid()),
                    StartTime = x.LessonSlot?.StartTime ?? new TimeOnly(),
                    EndTime = x.LessonSlot?.EndTime ?? new TimeOnly()
                },
                Lecturers = x.Lecturers?.Select(l => new Lecturer
                {
                    Id = l.Id == Guid.Empty ? Guid.NewGuid() : l.Id,
                    Name = l.Name,
                    Surname = l.Surname,
                }).ToList() ?? new List<Lecturer>()
            }).ToList() ?? new List<Lesson>()
        };

        await _context.Schedules.AddAsync(schedule);
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
        var schedules = await _context.Schedules.ToListAsync();
        _context.Schedules.RemoveRange(schedules);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateScheduleAsync(ScheduleDto scheduleDto)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Lessons)
            .FirstOrDefaultAsync(s => s.Id == scheduleDto.Id);

        if (schedule != null)
        {
            schedule.UpdatedAt = scheduleDto.UpdateAt;
            schedule.CanHeadmanUpdate = scheduleDto.HeadmanUpdate;
            schedule.IsAutoUpdate = scheduleDto.IsAutoUpdate;

            if (schedule.Lessons.Any())
            {
                _context.Lessons.RemoveRange(schedule.Lessons);
            }

            schedule.Lessons = scheduleDto.Lessons?.Select(x => new Lesson
            {
                Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
                Day = x.Day,
                LessonType = x.LessonType,
                Subject = new Subject
                {
                    Id = x.Subject?.Id == Guid.Empty ? Guid.NewGuid() : (x.Subject?.Id ?? Guid.NewGuid()),
                    Name = x.Subject?.Name ?? string.Empty
                },
                LessonsSlot = new LessonsSlot
                {
                    Id = x.LessonSlot?.Id == Guid.Empty ? Guid.NewGuid() : (x.LessonSlot?.Id ?? Guid.NewGuid()),
                    StartTime = x.LessonSlot?.StartTime ?? new TimeOnly(),
                    EndTime = x.LessonSlot?.EndTime ?? new TimeOnly()
                },
                Lecturers = x.Lecturers?.Select(l => new Lecturer
                {
                    Id = l.Id == Guid.Empty ? Guid.NewGuid() : l.Id,
                    Name = l.Name,
                    Surname = l.Surname,
                }).ToList() ?? new List<Lecturer>()
            }).ToList() ?? new List<Lesson>();

            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateHeadmanRights(ScheduleHeadmanRightsUpdateDto dto)
    {
        var schedule = await _context.Schedules.FindAsync(dto.Id);
        if (schedule != null)
        {
            schedule.CanHeadmanUpdate = dto.CanHeadmanUpdate;
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

    public async Task SetScheduleAutoUpdate()
    {
        var schedules = await _context.Schedules.ToListAsync();
        foreach (var schedule in schedules)
        {
            schedule.IsAutoUpdate = true;
        }
        await _context.SaveChangesAsync();
    }

    public async Task SetScheduleAutoUpdateInterval(uint interval)
    {
        var schedules = await _context.Schedules.ToListAsync();
        foreach (var schedule in schedules)
        {
            schedule.UpdateInterval = interval;
        }
        await _context.SaveChangesAsync();
    }
}