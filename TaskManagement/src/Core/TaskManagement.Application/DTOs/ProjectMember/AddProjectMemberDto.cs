using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Application.DTOs.ProjectMember
{
    public class AddProjectMemberDto
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public int AccessLevel { get; set; }
    }
}
