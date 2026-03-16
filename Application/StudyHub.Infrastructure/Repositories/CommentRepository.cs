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
    
    public async Task<List<Comment>> GetCommentsAsync()
    {
        return await _context.Comments.ToListAsync();
    }

    public async Task<Guid> UpdateCommentAsync(Comment newComment)
    {
        var currentComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == newComment.Id);

        if (currentComment is not null)
        {
            currentComment.Description = newComment.Description;
            await _context.SaveChangesAsync();
        }

        return newComment.Id;
    }

    public async Task<Guid> CreateCommentAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
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