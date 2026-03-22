using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.Infrastructure.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly SDbContext _context;

    public FeedbackRepository(SDbContext context)
    {
        _context = context;
    }

    public async Task<Feedback?> GetFeedbackAsync(Guid id) 
    { 
        return await _context.Feedbacks.FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<List<Feedback>> GetFeedbacksAsync()
    {
        return await _context.Feedbacks.ToListAsync();
    }
    public async Task<Guid> AddFeedbackAsync(Feedback feedback)
    {
        await _context.Feedbacks.AddAsync(feedback);
        await _context.SaveChangesAsync();
        
        return feedback.Id;
    }
    public async Task<Guid> UpdateFeedbackAsync(Feedback feedback)
    {
        var userFeedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == feedback.Id) ?? throw new Exception("Feedback not found");
        
        userFeedback.Category = feedback.Category;
        userFeedback.Status = feedback.Status;
        userFeedback.UpdatedAt = DateTime.UtcNow;

        if (feedback.Status == Status.Resolved)
        {
            userFeedback.ResolvedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return userFeedback.Id;
    }
}