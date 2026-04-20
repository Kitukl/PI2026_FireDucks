using MediatR;

namespace StudyHub.Core.Schedules.Commands;

public record UpdateGlobalScheduleSettingsRequest(bool IsAutoUpdate, bool AllowLeaders, uint IntervalDays) : IRequest;