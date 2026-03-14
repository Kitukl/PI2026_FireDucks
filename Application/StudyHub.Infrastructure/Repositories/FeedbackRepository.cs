using StudyHub.Core.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    public Task<IEnumerable<Feedback>> GetFeedbacksAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateFeedbackAsync(Feedback feedback)
    {
        throw new NotImplementedException();
    }
}