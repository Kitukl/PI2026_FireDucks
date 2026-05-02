using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Domain.Entities;

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
}