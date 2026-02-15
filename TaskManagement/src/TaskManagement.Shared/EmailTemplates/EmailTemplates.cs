using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Shared.EmailTemplates
{
    public static class EmailTemplates
    {
        public static string GetTaskAssignedTemplate(string recipientName, string taskTitle, string projectName)
        {
            return $@"
            <h2>Task Assigned</h2>
            <p>Hi {recipientName},</p>
            <p>You have been assigned to task: <strong>{taskTitle}</strong></p>
            <p>Project: <strong>{projectName}</strong></p>
            <p>Please log in to the system to view more details.</p>
        ";
        }

        public static string GetTaskDueReminderTemplate(string recipientName, string taskTitle, DateTime dueDate)
        {
            return $@"
            <h2>Task Due Reminder</h2>
            <p>Hi {recipientName},</p>
            <p>Your task <strong>{taskTitle}</strong> is due on <strong>{dueDate:dd/MM/yyyy}</strong></p>
            <p>Please complete it on time.</p>
        ";
        }

        public static string GetTaskStatusChangedTemplate(string recipientName, string taskTitle, string oldStatus, string newStatus)
        {
            return $@"
            <h2>Task Status Changed</h2>
            <p>Hi {recipientName},</p>
            <p>Task <strong>{taskTitle}</strong> status has changed from <strong>{oldStatus}</strong> to <strong>{newStatus}</strong></p>
        ";
        }

        public static string GetCommentMentionTemplate(string recipientName, string taskTitle, string commenterName, string commentContent)
        {
            return $@"
            <h2>You Were Mentioned in a Comment</h2>
            <p>Hi {recipientName},</p>
            <p>{commenterName} mentioned you in task <strong>{taskTitle}</strong></p>
            <p>Comment: <em>{commentContent}</em></p>
        ";
        }

        public static string GetProjectInvitationTemplate(string recipientName, string projectName, string inviterName)
        {
            return $@"
            <h2>Project Invitation</h2>
            <p>Hi {recipientName},</p>
            <p>{inviterName} invited you to project <strong>{projectName}</strong></p>
            <p>Log in to accept the invitation.</p>
        ";
        }
    }
}
