using StudyHub.Domain.Entities;

namespace StudyHub.Core.Feedbacks.Interfaces;

public interface IFeedbackRepository
{
    Task<Feedback?> GetFeedbackAsync(Guid Id);
    Task<List<Feedback>> GetFeedbacksAsync();
    Task<Guid> AddFeedbackAsync(Feedback feedback);
    Task<Guid> UpdateFeedbackAsync(Feedback feedback);
}