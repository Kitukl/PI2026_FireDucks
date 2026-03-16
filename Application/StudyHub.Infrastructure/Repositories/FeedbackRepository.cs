using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Statistics.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly StudyHubDbContext _context;

    public FeedbackRepository(StudyHubDbContext context)
    {
        _context = context;
    }

    public async Task<Feedback?> GetFeedbackAsync(Guid Id) 
    { 
        return await _context.Feedbacks.FindAsync(Id);
    }
    public async Task<List<Feedback>> GetFeedbacksAsync()
    {
        return await _context.Feedbacks.ToListAsync();
    }
    public async Task<Guid> AddFeedbackAsync(Feedback feedback)
    {
        await _context.Feedbacks.AddAsync(feedback);
        return feedback.Id;
    }
    public async Task<Guid> UpdateFeedbackAsync(Feedback feedback)
    {
        var userFeedback = await _context.Feedback.FirstOrDefaultAsync(f => f.Id == feedback.Id);

        if (userFeedback != null)
        {
            userFeedback.Category = feedback.Category;
            userFeedback.Status = feedback.Status;
            userFeedback.UpdatedAt = DateTime.UtcNow;

            if (feedback.Status == Status.Resolved)
            {
                userFeedback.ResolvedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        return userFeedback.Id.ToString();
    }
}