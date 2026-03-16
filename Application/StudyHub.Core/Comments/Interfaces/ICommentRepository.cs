using StudyHub.Domain.Entities;

namespace StudyHub.Core.Comments.Interfaces;

public interface ICommentRepository
{
    public Task<List<Comment>> GetCommentsAsync();
    public Task<Guid> UpdateCommentAsync(Comment comment);
    public Task<Guid> CreateCommentAsync(Comment comment);
    public Task<Guid> DeleteCommentAsync(Guid id);
}