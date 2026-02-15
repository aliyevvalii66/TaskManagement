using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using TaskManagement.Application.Services;
using TaskManagement.Shared.EmailTemplates;

namespace TaskManagement.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _smtpServer = _configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            _senderEmail = _configuration["EmailSettings:SenderEmail"];
            _senderPassword = _configuration["EmailSettings:SenderPassword"];
        }

        public async Task<bool> SendTaskAssignedEmailAsync(string recipientEmail, string recipientName, string taskTitle, string projectName)
        {
            try
            {
                _logger.LogInformation("Sending task assigned email to {RecipientEmail}", recipientEmail);

                var subject = $"Task Assigned: {taskTitle}";
                var body = EmailTemplates.GetTaskAssignedTemplate(recipientName, taskTitle, projectName);

                await SendEmailAsync(recipientEmail, subject, body);

                _logger.LogInformation("Task assigned email sent successfully to {RecipientEmail}", recipientEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending task assigned email to {RecipientEmail}", recipientEmail);
                return false;
            }
        }

        public async Task<bool> SendTaskDueReminderEmailAsync(string recipientEmail, string recipientName, string taskTitle, DateTime dueDate)
        {
            try
            {
                _logger.LogInformation("Sending task due reminder email to {RecipientEmail}", recipientEmail);

                var subject = $"Task Due Reminder: {taskTitle}";
                var body = EmailTemplates.GetTaskDueReminderTemplate(recipientName, taskTitle, dueDate);

                await SendEmailAsync(recipientEmail, subject, body);

                _logger.LogInformation("Task due reminder email sent successfully to {RecipientEmail}", recipientEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending task due reminder email to {RecipientEmail}", recipientEmail);
                return false;
            }
        }

        public async Task<bool> SendTaskStatusChangedEmailAsync(string recipientEmail, string recipientName, string taskTitle, string oldStatus, string newStatus)
        {
            try
            {
                _logger.LogInformation("Sending task status changed email to {RecipientEmail}", recipientEmail);

                var subject = $"Task Status Updated: {taskTitle}";
                var body = EmailTemplates.GetTaskStatusChangedTemplate(recipientName, taskTitle, oldStatus, newStatus);

                await SendEmailAsync(recipientEmail, subject, body);

                _logger.LogInformation("Task status changed email sent successfully to {RecipientEmail}", recipientEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending task status changed email to {RecipientEmail}", recipientEmail);
                return false;
            }
        }

        public async Task<bool> SendCommentMentionEmailAsync(string recipientEmail, string recipientName, string taskTitle, string commenterName, string commentContent)
        {
            try
            {
                _logger.LogInformation("Sending comment mention email to {RecipientEmail}", recipientEmail);

                var subject = $"You were mentioned in: {taskTitle}";
                var body = EmailTemplates.GetCommentMentionTemplate(recipientName, taskTitle, commenterName, commentContent);

                await SendEmailAsync(recipientEmail, subject, body);

                _logger.LogInformation("Comment mention email sent successfully to {RecipientEmail}", recipientEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending comment mention email to {RecipientEmail}", recipientEmail);
                return false;
            }
        }

        public async Task<bool> SendProjectInvitationEmailAsync(string recipientEmail, string recipientName, string projectName, string inviterName)
        {
            try
            {
                _logger.LogInformation("Sending project invitation email to {RecipientEmail}", recipientEmail);

                var subject = $"Project Invitation: {projectName}";
                var body = EmailTemplates.GetProjectInvitationTemplate(recipientName, projectName, inviterName);

                await SendEmailAsync(recipientEmail, subject, body);

                _logger.LogInformation("Project invitation email sent successfully to {RecipientEmail}", recipientEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending project invitation email to {RecipientEmail}", recipientEmail);
                return false;
            }
        }

        private async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
