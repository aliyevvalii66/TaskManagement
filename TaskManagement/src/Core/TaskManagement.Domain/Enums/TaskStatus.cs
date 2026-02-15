using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Domain.Enums
{
    public enum TaskStatus
    {
        Backlog = 0,
        Todo = 1,
        InProgress = 2,
        InReview = 3,
        Done = 4,
        Closed = 5
    }
}
