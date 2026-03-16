using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly SDbContext _context;

    public LessonRepository(SDbContext context)
    {
        _context = context;
    }

    public async Task<LessonDto?> GetById(Guid id)
    {
        return await _context.Lessons
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new LessonDto
            {
                Id = l.Id,
                Day = l.Day,
                LessonType = l.LessonType,
                Subject = new SubjectDto { Id = l.Subject.Id, Name = l.Subject.Name },
                LessonSlot = new LessonSlotDto { Id = l.LessonsSlot.Id, StartTime = l.LessonsSlot.StartTime, EndTime = l.LessonsSlot.EndTime },
                Lecturers = l.Lecturers.Select(lec => new LecturerDto { Id = lec.Id, Name = lec.Name, Surname = lec.Surname }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<LessonDto?>> GetAll()
    {
        return await _context.Lessons
            .AsNoTracking()
            .Select(l => new LessonDto
            {
                Id = l.Id,
                Day = l.Day,
                LessonType = l.LessonType,
                Subject = new SubjectDto { Id = l.Subject.Id, Name = l.Subject.Name }
            })
            .ToListAsync();
    }

    public async Task AddLesson(LessonDto lessonDto)
    {
        var lesson = new Lesson
        {
            Id = lessonDto.Id == Guid.Empty ? Guid.NewGuid() : lessonDto.Id,
            Day = lessonDto.Day,
            LessonType = lessonDto.LessonType,
            Lecturers = lessonDto.Lecturers.Select(x => new Lecturer
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
            }).ToList(),
            Subject = new Subject { Id = lessonDto.Subject.Id, Name = lessonDto.Subject.Name},
            LessonsSlot = new LessonsSlot
            {
                Id = lessonDto.Id,
                StartTime = lessonDto.LessonSlot.StartTime,
                EndTime = lessonDto.LessonSlot.EndTime
            }

        };

        await _context.Lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLesson(LessonDto lessonDto)
    {
        var lesson = await _context.Lessons.FindAsync(lessonDto.Id);
        if (lesson != null)
        {
            lesson.Day = lessonDto.Day;
            lesson.LessonType = lessonDto.LessonType;
            if ( lesson.Lecturers.Any() )
            {
                _context.Lecturers.RemoveRange(lesson.Lecturers);
            }

            lesson.Lecturers = lessonDto.Lecturers.Select(x => new Lecturer
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
            }).ToList();

            lesson.Subject = new Subject { Id = lessonDto.Subject.Id, Name = lessonDto.Subject.Name };
            lesson.LessonsSlot = new LessonsSlot
            {
                Id = lessonDto.Id,
                StartTime = lessonDto.LessonSlot.StartTime,
                EndTime = lessonDto.LessonSlot.EndTime
            };

            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteLesson(Guid id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson != null)
        {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }
    }
}