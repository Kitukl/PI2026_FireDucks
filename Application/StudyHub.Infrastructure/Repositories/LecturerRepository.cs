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

    public async Task<LecturerDto?> GetById(Guid id)
    {
        return await _context.Lecturers
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new LecturerDto
            {
                Id = l.Id,
                Name = l.Name,
                Surname = l.Surname,
                Lessons = l.Lessons.Select(les => new SubjectDto
                {
                    Id = les.Subject.Id,
                    Name = les.Subject.Name
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<LecturerDto?>> GetAll()
    {
        return await _context.Lecturers
            .AsNoTracking()
            .Select(l => new LecturerDto
            {
                Id = l.Id,
                Name = l.Name,
                Surname = l.Surname
            })
            .ToListAsync();
    }

    public async Task AddLecturer(LecturerDto lecturerDto)
    {
        var lecturer = new Lecturer
        {
            Id = lecturerDto.Id == Guid.Empty ? Guid.NewGuid() : lecturerDto.Id,
            Name = lecturerDto.Name,
            Surname = lecturerDto.Surname
        };

        await _context.Lecturers.AddAsync(lecturer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLecturer(LecturerDto lecturerDto)
    {
        var lecturer = await _context.Lecturers.FindAsync(lecturerDto.Id);
        if (lecturer != null)
        {
            lecturer.Name = lecturerDto.Name;
            lecturer.Surname = lecturerDto.Surname;

            _context.Lecturers.Update(lecturer);
            await _context.SaveChangesAsync();
        }
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