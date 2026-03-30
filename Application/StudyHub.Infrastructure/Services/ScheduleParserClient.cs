using System.Net.Http.Json;
using StudyHub.Core.DTOs.Parser;
using StudyHub.Core.Services;

namespace StudyHub.Infrastructure.Services
{
    public class ScheduleParserClient : IScheduleParserClient
    {
        private readonly HttpClient _httpClient;

        public ScheduleParserClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ParsedScheduleResponse?> ParseScheduleAsync(string groupName, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<ParsedScheduleResponse>(
                $"/api/v1/schedule/parse?group="+groupName,
                cancellationToken);
        }
    }
}