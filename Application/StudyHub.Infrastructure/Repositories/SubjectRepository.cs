using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly SDbContext _context;

    public SubjectRepository(SDbContext context)
    {
        _context = context;
    }

    public async Task<Subject?> GetById(Guid id)
    {
        return await _context.Subjects.FindAsync(id);
    }

    public async Task<List<Subject>> GetAll()
    {
        return await _context.Subjects
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddSubject(Subject subject)
    {
        var dbSubject = await _context.Subjects.FirstOrDefaultAsync(x => ( subject.Id == x.Id || subject.Name == x.Name )); 
        if (dbSubject == null)
        {
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            return;
        }
        else
        {
            dbSubject.Name = subject.Name;
            if (subject.Lessons != null)
            {
                dbSubject.Lessons.Clear();
                foreach (var lesson in subject.Lessons)
                {
                    var dbLesson = await _context.Lessons.FindAsync(lesson.Id);
                    dbSubject.Lessons.Add(dbLesson ?? lesson);
                }
            }

            if (subject.Tasks != null)
            {
                dbSubject.Tasks.Clear();
                foreach (var task in subject.Tasks)
                {
                    var dbTask = await _context.Tasks.FindAsync(task.Id);
                    dbSubject.Tasks.Add(dbTask ?? task);
                }
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateSubject(Subject subject)
    {
        var dbSubject = await _context.Subjects.FindAsync(subject.Id);
        if (dbSubject != null)
        {
            dbSubject.Name = subject.Name;
            if (subject.Lessons != null)
            {
                dbSubject.Lessons.Clear();
                foreach(var lesson in subject.Lessons)
                {
                    var dbLesson = await _context.Lessons.FindAsync(lesson.Id);
                    dbSubject.Lessons.Add(dbLesson ?? lesson);
                }
            }

            if (subject.Tasks != null)
            {
                dbSubject.Tasks.Clear();
                foreach (var task in subject.Tasks)
                {
                    var dbTask = await _context.Tasks.FindAsync(task.Id);
                    dbSubject.Tasks.Add(dbTask ?? task);
                }
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteSubject(Guid id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject != null)
        {
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }
    }
}