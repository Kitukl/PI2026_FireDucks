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

    public async Task<SubjectDto?> GetById(Guid id)
    {
        return await _context.Subjects
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<SubjectDto?>> GetAll()
    {
        return await _context.Subjects
            .AsNoTracking()
            .Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();
    }

    public async Task AddSubject(SubjectDto subjectDto)
    {
        var subject = new Subject
        {
            Id = subjectDto.Id == Guid.Empty ? Guid.NewGuid() : subjectDto.Id,
            Name = subjectDto.Name
        };

        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSubject(SubjectDto subjectDto)
    {
        var subject = await _context.Subjects.FindAsync(subjectDto.Id);
        if (subject != null)
        {
            subject.Name = subjectDto.Name;
            _context.Subjects.Update(subject);
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