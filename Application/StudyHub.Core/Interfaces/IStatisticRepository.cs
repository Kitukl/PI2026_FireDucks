using StudyHub.Domain.Entities;

namespace StudyHub.Core.Interfaces;

public interface IStatisticRepository
{
    Task<Statistic?> GetRecentStatisticAsync();
}