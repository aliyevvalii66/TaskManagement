using System.Net.Mail;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities
{
    public class Task : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedToId { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? EstimatedHours { get; set; }
        public int? ActualHours { get; set; }
        public Guid? ParentTaskId { get; set; }

        public Project Project { get; set; }
        public User AssignedTo { get; set; }
        public Task ParentTask { get; set; }
        public ICollection<Task> SubTasks { get; set; } = new List<Task>();
        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    }
}
