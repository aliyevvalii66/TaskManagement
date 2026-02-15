using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Application.DTOs.Project
{
    public class UpdateProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
