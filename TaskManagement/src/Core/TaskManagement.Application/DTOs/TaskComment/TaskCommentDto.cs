using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Application.DTOs.TaskComment
{
    public class TaskCommentDto
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public DateTime CommentedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
