using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Core.Subjects.Interfaces;
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

    public async Task<LessonsSlot?> GetById(Guid id)
    {
        return await _context.LessonsSlots.FindAsync(id);
    }

    public async Task<List<LessonsSlot>> GetAll()
    {
        return await _context.LessonsSlots
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddLessonSlot(LessonsSlot lessonSlot)
    {
        await _context.LessonsSlots.AddAsync(lessonSlot);
        await _context.SaveChangesAsync();
        
    }

    public async Task UpdateLessonSlot(LessonsSlot lessonSlot)
    {
        var dbSlot = await _context.LessonsSlots.FindAsync(lessonSlot.Id);
        if (dbSlot != null)
        {
            dbSlot.StartTime = lessonSlot.StartTime;
            dbSlot.EndTime = lessonSlot.EndTime;
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