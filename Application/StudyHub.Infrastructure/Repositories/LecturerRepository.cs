using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Repositories;

public class LecturerRepository : ILecturerRepository
{
    private readonly SDbContext _context;

    public LecturerRepository(SDbContext context)
    {
        _context = context;
    }

    public async Task<Lecturer?> GetById(Guid id)
    {
        return await _context.Lecturers
            .Include(l => l.Lessons)
                .ThenInclude(lesson => lesson.Subject)
            .Include(l => l.Lessons)
                .ThenInclude(lesson => lesson.LessonsSlot)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<Lecturer>> GetAll()
    {
        return await _context.Lecturers
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddLecturer(Lecturer lecturer)
    {
        if (lecturer.Lessons != null && lecturer.Lessons.Any())
        {
            var processedLessons = new List<Lesson>();

            foreach (var incomingLesson in lecturer.Lessons)
            {
                var dbLesson = await _context.Lessons.FindAsync(incomingLesson.Id);

                if (dbLesson != null)
                {
                    processedLessons.Add(dbLesson);
                }
                else
                {
                    if (incomingLesson.Subject != null)
                    {
                        var dbSubject = await _context.Subjects.FindAsync(incomingLesson.Subject.Id);
                        if (dbSubject != null) incomingLesson.Subject = dbSubject;
                    }

                    if (incomingLesson.LessonsSlot != null)
                    {
                        var dbSlot = await _context.LessonsSlots.FindAsync(incomingLesson.LessonsSlot.Id);
                        if (dbSlot != null) incomingLesson.LessonsSlot = dbSlot;
                    }

                    processedLessons.Add(incomingLesson);
                }
            }

            lecturer.Lessons = processedLessons;
        }

        await _context.Lecturers.AddAsync(lecturer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLecturer(Lecturer lecturer)
    {
        var existingLecturer = await _context.Lecturers
            .Include(l => l.Lessons)
            .FirstOrDefaultAsync(l => l.Id == lecturer.Id);

        if (existingLecturer == null) return;

        existingLecturer.Name = lecturer.Name;
        existingLecturer.Surname = lecturer.Surname;

        if (lecturer.Lessons != null)
        {
            // Очищаємо поточні зв'язки лектора з уроками
            existingLecturer.Lessons.Clear();

            foreach (var lesson in lecturer.Lessons)
            {
                // 1. Шукаємо, чи існує такий УРОК у базі
                var dbLesson = await _context.Lessons.FindAsync(lesson.Id);

                if (dbLesson != null)
                {
                    // Якщо урок існуючий, просто додаємо його до колекції лектора
                    existingLecturer.Lessons.Add(dbLesson);
                }
                else
                {
                    // 2. Якщо це НОВИЙ урок (наприклад, доданий через JS на формі)
                    // Нам потрібно переконатися, що EF не намагається створити новий Subject/Slot

                    if (lesson.Subject != null)
                    {
                        // Кажемо EF: "Цей предмет уже в базі, не створюй його"
                        _context.Entry(lesson.Subject).State = EntityState.Unchanged;
                    }

                    if (lesson.LessonsSlot != null)
                    {
                        // Кажемо EF: "Цей слот уже в базі, не створюй його"
                        _context.Entry(lesson.LessonsSlot).State = EntityState.Unchanged;
                    }

                    // Тепер додаємо новий урок. EF створить ТІЛЬКИ запис у Lessons та зв'язок
                    existingLecturer.Lessons.Add(lesson);
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteLecturer(Guid id)
    {
        var lecturer = await _context.Lecturers.FindAsync(id);
        if (lecturer != null)
        {
            _context.Lecturers.Remove(lecturer);
            await _context.SaveChangesAsync();
        }
    }
}