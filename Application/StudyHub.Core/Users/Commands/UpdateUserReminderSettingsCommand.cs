using MediatR;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Users.Commands;

public class UpdateUserReminderSettingsCommand : IRequest<UpdateUserReminderSettingsResult>
{
    public Guid? UserId { get; set; }
    public bool IsNotified { get; set; }
    public uint Offset { get; set; }
    public TimeType TimeType { get; set; }
}

public class UpdateUserReminderSettingsResult
{
    public bool IsSuccess { get; set; }
}

public class UpdateUserReminderSettingsCommandHandler : IRequestHandler<UpdateUserReminderSettingsCommand, UpdateUserReminderSettingsResult>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserReminderSettingsCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UpdateUserReminderSettingsResult> Handle(UpdateUserReminderSettingsCommand request, CancellationToken cancellationToken)
    {
        if (!request.UserId.HasValue)
        {
            return new UpdateUserReminderSettingsResult { IsSuccess = false };
        }

        var user = await _userRepository.GetUserById(request.UserId.Value);

        user.IsNotified = request.IsNotified;
        if (request.IsNotified)
        {
            if (user.Reminder is null)
            {
                user.Reminder = new Reminder();
            }

            user.Reminder.ReminderOffset = request.Offset;
            user.Reminder.TimeType = request.TimeType;
        }

        await _userRepository.Update(user);
        return new UpdateUserReminderSettingsResult { IsSuccess = true };
    }
}
