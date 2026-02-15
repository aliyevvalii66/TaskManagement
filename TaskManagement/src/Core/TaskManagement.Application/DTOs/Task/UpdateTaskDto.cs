using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Application.DTOs.Task
{
    public class UpdateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? EstimatedHours { get; set; }
    }
}
