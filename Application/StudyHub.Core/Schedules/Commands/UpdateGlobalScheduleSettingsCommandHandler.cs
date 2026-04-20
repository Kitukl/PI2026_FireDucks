using MediatR;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.Core.Schedules.Commands;

public class UpdateGlobalScheduleSettingsCommandHandler : IRequestHandler<UpdateGlobalScheduleSettingsRequest>
{
    private readonly IScheduleRepository _scheduleRepository;

    public UpdateGlobalScheduleSettingsCommandHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task Handle(UpdateGlobalScheduleSettingsRequest request, CancellationToken cancellationToken)
    {
        await _scheduleRepository.UpdateGlobalSettings(
            request.IsAutoUpdate,
            request.AllowLeaders,
            request.IntervalDays,
            DateTime.UtcNow);
    }
}