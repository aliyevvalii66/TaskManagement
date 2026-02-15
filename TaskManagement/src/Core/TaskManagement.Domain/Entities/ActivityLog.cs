using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities
{
    public class ActivityLog : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid? TaskId { get; set; }
        public Guid? ProjectId { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ActionAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public Domain.Entities.Task Task { get; set; }
        public Project Project { get; set; }
    }
}
