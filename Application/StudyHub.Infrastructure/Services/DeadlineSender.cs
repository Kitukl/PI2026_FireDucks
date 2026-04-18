using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudyHub.Domain.Entities;
using StudyHub.Core.Notifications.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Services;

public class DeadlineSender : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DeadlineSender> _logger;

    public DeadlineSender(IServiceProvider services, ILogger<DeadlineSender> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Reminder Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndSendRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking task deadlines.");
            }

            // Чекаємо 1 годину перед наступною перевіркою
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task CheckAndSendRemindersAsync(CancellationToken stoppingToken)
    {
        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SDbContext>(); 
        var announcementService = scope.ServiceProvider.GetRequiredService<IGlobalAnnouncementService>();

        var now = DateTime.UtcNow;

        var activeUsers = await context.Users
            .Include(u => u.Reminder)
            .Where(u => u.IsNotified && u.Reminder != null)
            .ToListAsync(stoppingToken);

        foreach (var user in activeUsers)
        {
            DateTime thresholdDate = CalculateThreshold(now, user.Reminder.ReminderOffset, user.Reminder.TimeType);
            var upcomingTasks = await context.Tasks 
                .Where(t => t.User.Id == user.Id 
                            && t.Deadline > now 
                            && t.Deadline <= thresholdDate)
                .ToListAsync(stoppingToken);

            foreach (var task in upcomingTasks)
            {
                _logger.LogInformation("Sending reminder to {Email} for task {TaskId}", user.Email, task.Id);

                await announcementService.SendReminderEmailAsync(
                    user.Email!,
                    "Нагадування про дедлайн",
                    task.Description ?? "Завдання без опису",
                    task.Deadline,
                    stoppingToken);
            }
        }
        
        await context.SaveChangesAsync(stoppingToken);
    }

    private DateTime CalculateThreshold(DateTime from, uint offset, TimeType type)
    {
        return type switch
        {
            TimeType.Hour => from.AddHours(offset),
            TimeType.Day => from.AddDays(offset),
            TimeType.Week => from.AddDays(offset * 7),
            _ => from.AddDays(offset)
        };
    }
}