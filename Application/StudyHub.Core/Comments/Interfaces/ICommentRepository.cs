using StudyHub.Domain.Entities;

namespace StudyHub.Core.Comments.Interfaces;

public interface ICommentRepository
{
    public Task<List<Comment>> GetCommentsAsync(Guid taskid);
    public Task<Guid> CreateCommentAsync(Comment comment);
    public Task<Guid> DeleteCommentAsync(Guid id);
}