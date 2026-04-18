using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using StudyHub.Core.Notifications.Interfaces;

namespace StudyHub.Infrastructure.Notifications;

public class GlobalAnnouncementService : IGlobalAnnouncementService
{
    private const string ApiKeyConfigPath = "SendGrid:ApiKey";
    private const string FromEmailConfigPath = "SendGrid:FromEmail";
    private const string FromNameConfigPath = "SendGrid:FromName";

    private readonly IConfiguration _configuration;
    private readonly ILogger<GlobalAnnouncementService> _logger;

    public GlobalAnnouncementService(
        IConfiguration configuration,
        ILogger<GlobalAnnouncementService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendGlobalAnnouncementAsync(
        IReadOnlyCollection<string> recipients,
        string subject,
        string description,
        CancellationToken cancellationToken = default)
    {
        if (recipients.Count == 0)
        {
            return;
        }

        var apiKey = _configuration[ApiKeyConfigPath];
        var fromEmail = _configuration[FromEmailConfigPath];
        var fromName = _configuration[FromNameConfigPath] ?? "StudyHub";

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(fromEmail))
        {
            throw new InvalidOperationException("SendGrid configuration is missing.");
        }

        var validRecipients = recipients
            .Where(email => !string.IsNullOrWhiteSpace(email) && email.Contains('@'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(email => new EmailAddress(email))
            .ToList();

        if (validRecipients.Count == 0)
        {
            return;
        }

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var message = MailHelper.CreateSingleEmailToMultipleRecipients(
            from,
            validRecipients,
            subject,
            plainTextContent: description,
            htmlContent: $"<p>{System.Net.WebUtility.HtmlEncode(description).Replace("\n", "<br />")}</p>");
        message.SetReplyTo(from);

        var response = await client.SendEmailAsync(message, cancellationToken);
        if ((int)response.StatusCode >= 400)
        {
            var responseBody = await response.Body.ReadAsStringAsync(cancellationToken);
            var details = ExtractSendGridErrorMessage(responseBody);
            _logger.LogError(
                "SendGrid announcement failed. StatusCode={StatusCode}. Body={Body}",
                response.StatusCode,
                responseBody);

            if (string.IsNullOrWhiteSpace(details))
            {
                throw new InvalidOperationException($"Failed to send announcement email. SendGrid status: {(int)response.StatusCode}.");
            }

            throw new InvalidOperationException(
                $"Failed to send announcement email. SendGrid status: {(int)response.StatusCode}. {details}");
        }

        _logger.LogInformation(
            "Global announcement sent via SendGrid. Recipients: {RecipientCount}. Subject: {Subject}",
            validRecipients.Count,
            subject);
    }

    private static string ExtractSendGridErrorMessage(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return string.Empty;
        }

        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            if (!doc.RootElement.TryGetProperty("errors", out var errors) || errors.ValueKind != JsonValueKind.Array)
            {
                return responseBody;
            }

            var messages = errors.EnumerateArray()
                .Select(error =>
                {
                    if (error.TryGetProperty("message", out var messageElement))
                    {
                        return messageElement.GetString();
                    }

                    return null;
                })
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return messages.Count == 0 ? responseBody : string.Join("; ", messages);
        }
        catch
        {
            return responseBody;
        }
    }
    
    public async Task SendReminderEmailAsync(
        string recipientEmail,
        string subject,
        string taskTitle,
        DateTime deadline,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail)) return;

        var apiKey = _configuration[ApiKeyConfigPath];
        var fromEmail = _configuration[FromEmailConfigPath];
        var fromName = _configuration[FromNameConfigPath] ?? "StudyHub";

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(fromEmail))
        {
            throw new InvalidOperationException("SendGrid configuration is missing.");
        }

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(recipientEmail);

        string plainTextContent = $"Нагадування про дедлайн: таск '{taskTitle}' має бути виконаний до {deadline:dd.MM.yyyy HH:mm}.";
        string htmlContent = $@"
            <div style='font-family: sans-serif; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                <h2 style='color: #2d5a27;'>Нагадування про дедлайн!</h2>
                <p>Зверніть увагу, що термін виконання вашого завдання добігає кінця:</p>
                <div style='background: #f9f9f9; padding: 15px; border-left: 5px solid #007bff;'>
                    <strong>Завдання:</strong> {System.Net.WebUtility.HtmlEncode(taskTitle)}<br/>
                    <strong>Дедлайн:</strong> {deadline:dd.MM.yyyy HH:mm}
                </div>
                <p style='margin-top: 20px; font-size: 12px; color: #777;'>
                    Ви отримали цей лист, оскільки у вас увімкнені сповіщення в профілі StudyHub.
                </p>
            </div>";

        var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        message.SetReplyTo(from);

        var response = await client.SendEmailAsync(message, cancellationToken);

        if ((int)response.StatusCode >= 400)
        {
            var responseBody = await response.Body.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed to send reminder to {Email}. Status: {Status}. Body: {Body}", 
                recipientEmail, response.StatusCode, responseBody);
            throw new InvalidOperationException($"SendGrid failed to send reminder to {recipientEmail}");
        }

        _logger.LogInformation("Reminder sent to {Email} for task: {TaskTitle}", recipientEmail, taskTitle);
    }
}
