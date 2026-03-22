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

    public async Task<Lesson?> GetById(Guid id)
    {
        return await _context.Lessons
            .Include(l => l.Subject)
            .Include(l => l.LessonsSlot)
            .Include(l => l.Lecturers)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<Lesson>> GetAll()
    {
        return await _context.Lessons
            .Include(l => l.Subject)
            .Include(l => l.LessonsSlot)
            .Include(l => l.Lecturers)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddLesson(Lesson lesson)
    {
        if (lesson.Subject != null)
        {
            var dbSubject = await _context.Subjects.FindAsync(lesson.Subject.Id);
            if (dbSubject != null) lesson.Subject = dbSubject;
        }

        if (lesson.LessonsSlot != null)
        {
            var dbSlot = await _context.LessonsSlots.FindAsync(lesson.LessonsSlot.Id);
            if (dbSlot != null) lesson.LessonsSlot = dbSlot;
        }

        if (lesson.Lecturers != null && lesson.Lecturers.Any())
        {
            var processedLecturers = new List<Lecturer>();
            foreach (var lecturer in lesson.Lecturers)
            {
                var dbLecturer = await _context.Lecturers.FindAsync(lecturer.Id);
                processedLecturers.Add(dbLecturer ?? lecturer);
            }
            lesson.Lecturers = processedLecturers;
        }

        await _context.Lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLesson(Lesson lesson)
    {
        var existingLesson = await _context.Lessons
            .Include(l => l.Lecturers)
            .FirstOrDefaultAsync(l => l.Id == lesson.Id);

        if (existingLesson == null) return;

        existingLesson.Day = lesson.Day;
        existingLesson.LessonType = lesson.LessonType;

        if (lesson.Subject != null)
        {
            var dbSubject = await _context.Subjects.FindAsync(lesson.Subject.Id);
            existingLesson.Subject = dbSubject ?? lesson.Subject;
        }

        if (lesson.LessonsSlot != null)
        {
            var dbSlot = await _context.LessonsSlots.FindAsync(lesson.LessonsSlot.Id);
            existingLesson.LessonsSlot = dbSlot ?? lesson.LessonsSlot;
        }

        if (lesson.Lecturers != null)
        {
            existingLesson.Lecturers.Clear();
            foreach (var lecturer in lesson.Lecturers)
            {
                var dbLecturer = await _context.Lecturers.FindAsync(lecturer.Id);
                existingLesson.Lecturers.Add(dbLecturer ?? lecturer);
            }
        }

        await _context.SaveChangesAsync();
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