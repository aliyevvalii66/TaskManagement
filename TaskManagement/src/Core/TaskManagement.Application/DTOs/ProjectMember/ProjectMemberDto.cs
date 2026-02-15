using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.ProjectMember
{
    public class ProjectMemberDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public ProjectAccessLevel AccessLevel { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
