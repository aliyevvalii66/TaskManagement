using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities
{
    public class ProjectMember : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public ProjectAccessLevel AccessLevel { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public Project Project { get; set; }
        public User User { get; set; }
    }
}
