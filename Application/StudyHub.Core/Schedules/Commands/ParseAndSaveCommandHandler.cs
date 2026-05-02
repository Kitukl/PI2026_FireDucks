using MediatR;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Services;

namespace StudyHub.Core.Schedules.Commands
{
    public record ParseAndSaveScheduleCommand(string GroupName) : IRequest<bool>;

    public class ParseAndSaveScheduleCommandHandler : IRequestHandler<ParseAndSaveScheduleCommand, bool>
    {
        private readonly IScheduleRepository _repo;
        private readonly IScheduleParserClient _parserClient;

        public ParseAndSaveScheduleCommandHandler(IScheduleRepository repo, IScheduleParserClient parserClient)
        {
            _repo = repo;
            _parserClient = parserClient;
        }

        public async Task<bool> Handle(ParseAndSaveScheduleCommand request, CancellationToken cancellationToken)
        {
            var parsedData = await _parserClient.ParseScheduleAsync(request.GroupName, cancellationToken);

            if (parsedData == null || !parsedData.Lessons.Any())
                return false;

            await _repo.SyncParsedScheduleAsync(request.GroupName, parsedData, cancellationToken);

            return true;
        }
    }
}