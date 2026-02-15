using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.TaskComment;
using TaskManagement.Shared.Results;

namespace TaskManagement.Application.Services
{
    public interface ITaskCommentService
    {
        Task<Result<TaskCommentDto>> GetCommentByIdAsync(Guid id);
        Task<Result<IEnumerable<TaskCommentDto>>> GetTaskCommentsAsync(Guid taskId);
        Task<Result<TaskCommentDto>> AddCommentAsync(CreateTaskCommentDto createCommentDto, Guid userId);
        Task<Result> UpdateCommentAsync(Guid id, UpdateTaskCommentDto updateCommentDto);
        Task<Result> DeleteCommentAsync(Guid id);
    }
}
