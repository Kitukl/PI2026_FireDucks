using Application.Helpers;
using MediatR;
using StudyHub.Core.Statistics.Interfaces;
using StudyHub.Core.Users.Interfaces;
using TimeType = StudyHub.Domain.Entities.TimeType;

namespace StudyHub.Core.Users.Queries;

public class GetUserProfilePageQuery : IRequest<UserProfilePageDataDto>
{
    public Guid? UserId { get; set; }
}

public class UserProfilePageDataDto
{
    public string FullName { get; set; } = "Гість";
    public string PhotoUrl { get; set; } = "/images/no-photo.png";
    public bool IsNotified { get; set; } = true;
    public uint ReminderOffset { get; set; } = 2u;
    public TimeType ReminderTimeType { get; set; } = TimeType.Day;
    public Dictionary<int, double> MonthlyActivityPerMonth { get; set; } = [];
}

public class GetUserProfilePageQueryHandler : IRequestHandler<GetUserProfilePageQuery, UserProfilePageDataDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IStatisticRepository _statisticRepository;

    public GetUserProfilePageQueryHandler(IUserRepository userRepository, IStatisticRepository statisticRepository)
    {
        _userRepository = userRepository;
        _statisticRepository = statisticRepository;
    }

    public async Task<UserProfilePageDataDto> Handle(GetUserProfilePageQuery request, CancellationToken cancellationToken)
    {
        if (!request.UserId.HasValue)
        {
            return new UserProfilePageDataDto();
        }

        var user = await _userRepository.GetUserById(request.UserId.Value);
        var monthlyActivityPerMonth = await _statisticRepository.GetYearlyActivityAsync(
            request.UserId.Value,
            DateTime.UtcNow.Year,
            cancellationToken: cancellationToken);

        return new UserProfilePageDataDto
        {
            FullName = $"{user.Name} {user.Surname}".Trim(),
            PhotoUrl = UserProfileHelper.ResolvePhotoUrl(user.PhotoUrl),
            IsNotified = user.IsNotified,
            ReminderOffset = user.Reminder?.ReminderOffset ?? 2u,
            ReminderTimeType = user.Reminder?.TimeType ?? TimeType.Day,
            MonthlyActivityPerMonth = monthlyActivityPerMonth
        };
    }
}
