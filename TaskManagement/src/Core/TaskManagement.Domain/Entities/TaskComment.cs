using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities
{
    public class TaskComment : BaseEntity
    {
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;

        public Domain.Entities.Task Task { get; set; }
        public User User { get; set; }
    }
}
