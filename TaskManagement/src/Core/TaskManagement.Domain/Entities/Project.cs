using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsArchived { get; set; } = false;

        public User Owner { get; set; }
        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
        public ICollection<Domain.Entities.Task> Tasks { get; set; } = new List<Domain.Entities.Task>();
    }
}
