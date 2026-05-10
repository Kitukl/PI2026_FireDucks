using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly SDbContext _context;

    public CommentRepository(SDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Comment>> GetCommentsAsync(Guid taskid)
    {
        return await _context.Comments
            .Where(c => c.Task != null && c.Task.Id == taskid)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Comment>> GetFeedbackCommentsAsync(Guid feedbackId)
    {
        return await _context.Comments
            .Where(c => c.Feedback != null && c.Feedback.Id == feedbackId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Guid> CreateCommentAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return comment.Id;
    }

    public async Task<Guid> DeleteCommentAsync(Guid id)
    {
        await _context.Comments
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();
        return id;
    }
}