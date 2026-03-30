using StudyHub.Core.DTOs.Parser;

namespace StudyHub.Core.Services
{
    public interface IScheduleParserClient
    {
        Task<ParsedScheduleResponse?> ParseScheduleAsync(string groupName, CancellationToken cancellationToken = default);
    }
}
