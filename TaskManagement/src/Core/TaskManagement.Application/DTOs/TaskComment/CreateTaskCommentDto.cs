using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Application.DTOs.TaskComment
{
    public class CreateTaskCommentDto
    {
        public Guid TaskId { get; set; }
        public string Content { get; set; }
    }
}
