using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.Infrastructure.Services
{
    public class ScheduleAutoUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        public ScheduleAutoUpdateService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var scheduleRepo = scope.ServiceProvider.GetRequiredService<IScheduleRepository>();

                    var allSchedules = await scheduleRepo.GetAll();
                    var schedulesToUpdate = allSchedules
                        .Where(s => s.IsAutoUpdate &&
                                   (DateTime.UtcNow - s.UpdatedAt).TotalDays >= 3)
                        .ToList();

                    foreach (var schedule in schedulesToUpdate)
                    {
                        await mediator.Send(new ParseAndSaveScheduleCommand(schedule.Group.Name), stoppingToken);
                    }
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}