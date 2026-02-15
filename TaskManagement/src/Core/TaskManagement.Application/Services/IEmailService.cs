using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Application.Services
{
    public interface IEmailService
    {
        Task<bool> SendTaskAssignedEmailAsync(string recipientEmail, string recipientName, string taskTitle, string projectName);
        Task<bool> SendTaskDueReminderEmailAsync(string recipientEmail, string recipientName, string taskTitle, DateTime dueDate);
        Task<bool> SendTaskStatusChangedEmailAsync(string recipientEmail, string recipientName, string taskTitle, string oldStatus, string newStatus);
        Task<bool> SendCommentMentionEmailAsync(string recipientEmail, string recipientName, string taskTitle, string commenterName, string commentContent);
        Task<bool> SendProjectInvitationEmailAsync(string recipientEmail, string recipientName, string projectName, string inviterName);
    }
}
