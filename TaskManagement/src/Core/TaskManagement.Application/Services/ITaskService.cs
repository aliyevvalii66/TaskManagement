using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Shared.Results;

namespace TaskManagement.Application.Services
{
    public interface ITaskService
    {
        Task<Result<TaskDto>> GetTaskByIdAsync(Guid id);
        Task<Result<IEnumerable<TaskDto>>> GetAllTasksAsync();
        Task<Result<PaginatedResponseDto<TaskDto>>> GetPagedTasksAsync(int pageNumber, int pageSize);
        Task<Result<IEnumerable<TaskDto>>> GetProjectTasksAsync(Guid projectId);
        Task<Result<IEnumerable<TaskDto>>> GetUserTasksAsync(Guid userId);
        Task<Result<TaskDto>> CreateTaskAsync(CreateTaskDto createTaskDto, Guid userId);
        Task<Result> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto);
        Task<Result> DeleteTaskAsync(Guid id);
        Task<Result> ChangeTaskStatusAsync(Guid id, int status);
        Task<Result> AssignTaskAsync(Guid id, Guid userId);
    }
}
