using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Application.DTOs.ActivityLog
{
    public class CreateActivityLogDto
    {
        public Guid UserId { get; set; }
        public Guid? TaskId { get; set; }
        public Guid? ProjectId { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
