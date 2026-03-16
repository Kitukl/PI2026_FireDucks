using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Domain.Entities;

using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Repositories;

public class LessonSlotRepository : ILessonSlotRepository
{
    private readonly SDbContext _context;

    public LessonSlotRepository(SDbContext context)
    {
        _context = context;
    }

    public async Task<LessonSlotDto?> GetById(Guid id)
    {
        return await _context.LessonsSlots
            .AsNoTracking()
            .Where(ls => ls.Id == id)
            .Select(ls => new LessonSlotDto
            {
                Id = ls.Id,
                StartTime = ls.StartTime,
                EndTime = ls.EndTime,
                Lessons = ls.Lessons.Select(l => new SubjectDto { Id = l.Subject.Id, Name = l.Subject.Name }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<LessonSlotDto?>> GetAll()
    {
        return await _context.LessonsSlots
            .AsNoTracking()
            .Select(ls => new LessonSlotDto
            {
                Id = ls.Id,
                StartTime = ls.StartTime,
                EndTime = ls.EndTime
            })
            .ToListAsync();
    }

    public async Task AddLessonSlot(LessonSlotDto slotDto)
    {
        var slot = new LessonsSlot
        {
            Id = slotDto.Id == Guid.Empty ? Guid.NewGuid() : slotDto.Id,
            StartTime = slotDto.StartTime,
            EndTime = slotDto.EndTime
        };

        await _context.LessonsSlots.AddAsync(slot);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLessonSlot(LessonSlotDto slotDto)
    {
        var slot = await _context.LessonsSlots.FindAsync(slotDto.Id);
        if (slot != null)
        {
            slot.StartTime = slotDto.StartTime;
            slot.EndTime = slotDto.EndTime;

            _context.LessonsSlots.Update(slot);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteLessonSlot(Guid id)
    {
        var slot = await _context.LessonsSlots.FindAsync(id);
        if (slot != null)
        {
            _context.LessonsSlots.Remove(slot);
            await _context.SaveChangesAsync();
        }
    }
}