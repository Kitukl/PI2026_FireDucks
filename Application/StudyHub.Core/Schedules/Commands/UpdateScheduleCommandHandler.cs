using MediatR;
using Microsoft.Extensions.Configuration;
using StudyHub.Core.DTOs;
using StudyHub.Core.Group;
using StudyHub.Core.Notifications.Interfaces;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Commands
{
    public record UpdateScheduleRequest(ScheduleDto dto) : IRequest;

    public class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleRequest>
    {
        private readonly IScheduleRepository _repo;
        private readonly IGlobalAnnouncementService _emailSender;
        private readonly IGroupRepository _groupRepository;
        private readonly IConfiguration _configuration;

        private const string SchedulePagePath = "SendGrid:SchedulePage";

        public UpdateScheduleCommandHandler(IScheduleRepository repo, IGlobalAnnouncementService emailSender, IGroupRepository groupRepository, IConfiguration configuration)
        {
            _repo = repo;
            _emailSender = emailSender;
            _groupRepository = groupRepository;
            _configuration = configuration;
        }

        public async Task Handle(UpdateScheduleRequest request, CancellationToken cancellationToken)
        {
            var scheduleUpdate = new Schedule
            {
                Id = request.dto.Id,
                IsAutoUpdate = request.dto.IsAutoUpdate,
                CanHeadmanUpdate = request.dto.LeaderUpdate,
                UpdatedAt = DateTime.UtcNow,
                Lessons = request.dto.Lessons?
                    .Where(l => l != null)
                    .Select(l => new Lesson { Id = l.Id })
                    .ToList() ?? new List<Lesson>()
            };

            await _repo.UpdateScheduleAsync(scheduleUpdate);

            var schedulePage = _configuration[SchedulePagePath];
            var groupWithUsers = await _groupRepository.GetGroupByIdAsync(request.dto.Group.Id);
            var recipientEmails = groupWithUsers?.Users.Select(c => c.Email).ToList();

            if (recipientEmails != null && recipientEmails.Any())
            {
                var groupName = request.dto.Group.Name;
                var updateTime = DateTime.UtcNow.AddHours(3).ToString("dd.MM.yyyy HH:mm");

                string subject = $"StudyHub: Оновлено розклад для {groupName}";
                
                string htmlContent = $@"
                    <div style='font-family: sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                        <h2 style='color: #2D3E50;'>Привіт! 👋</h2>
                        <p style='font-size: 16px; color: #555;'>У системі <strong>StudyHub</strong> з'явилися зміни в розкладі вашої групи.</p>
                        
                        <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #007bff; margin: 20px 0;'>
                            <p style='margin: 0;'><strong>Група:</strong> {groupName}</p>
                            <p style='margin: 5px 0 0 0;'><strong>Час оновлення:</strong> {updateTime}</p>
                        </div>

                        <p style='font-size: 14px; color: #777;'>Радимо перевірити актуальні пари в особистому кабінеті або мобільному додатку.</p>
                        
                        <div style='text-align: center; margin-top: 30px;'>
                            <a href='{schedulePage}' 
                               style='background-color: #007bff; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                               Переглянути розклад
                            </a>
                        </div>
                        
                        <hr style='margin-top: 40px; border: 0; border-top: 1px solid #eee;' />
                        <p style='font-size: 12px; color: #aaa; text-align: center;'>Це автоматичне повідомлення від StudyHub. Будь ласка, не відповідайте на нього.</p>
                    </div>";
                
                await _emailSender.SendGlobalAnnouncementAsync(
                    recipientEmails,
                    subject,
                    htmlContent,
                    cancellationToken);
            }
        }
    }
}
