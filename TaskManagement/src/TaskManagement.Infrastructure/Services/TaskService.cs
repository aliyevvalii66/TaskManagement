using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.DTOs.User;
using TaskManagement.Application.Repositories;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Enums;
using TaskManagement.Shared.Constants;
using TaskManagement.Shared.Results;

namespace TaskManagement.Infrastructure.Services
{
    public class TaskService : BaseService<Task, TaskDto>, ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        public TaskService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TaskService> logger, IEmailService emailService) : base(mapper,logger)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<Result<TaskDto>> GetTaskByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting task with id {TaskId}", id);
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task with id {TaskId} not found", id);
                return Result<TaskDto>.Failure(AppConstants.ErrorMessages.TaskNotFound);
            }

            var taskDto = _mapper.Map<TaskDto>(task);
            _logger.LogInformation("Task with id {TaskId} retrieved successfully", id);
            return Result<TaskDto>.Success(taskDto);
        }

        public async Task<Result<IEnumerable<TaskDto>>> GetAllTasksAsync()
        {
            _logger.LogInformation("Getting all tasks");
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var taskDtos = _mapper.Map<List<TaskDto>>(tasks);

            _logger.LogInformation("Retrieved {Count} tasks", taskDtos.Count);
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);
        }

        public async Task<Result<IEnumerable<TaskDto>>> GetProjectTasksAsync(Guid projectId)
        {
            _logger.LogInformation("Getting tasks for project {ProjectId}", projectId);
            var tasks = await _unitOfWork.Tasks.FindAsync(t => t.ProjectId == projectId);
            var taskDtos = _mapper.Map<List<TaskDto>>(tasks);

            _logger.LogInformation("Retrieved {Count} tasks for project {ProjectId}", taskDtos.Count, projectId);
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);
        }

        public async Task<Result<IEnumerable<TaskDto>>> GetUserTasksAsync(Guid userId)
        {
            _logger.LogInformation("Getting tasks assigned to user {UserId}", userId);
            var tasks = await _unitOfWork.Tasks.FindAsync(t => t.AssignedToId == userId);
            var taskDtos = _mapper.Map<List<TaskDto>>(tasks);

            _logger.LogInformation("Retrieved {Count} tasks assigned to user {UserId}", taskDtos.Count, userId);
            return Result<IEnumerable<TaskDto>>.Success(taskDtos);
        }

        public async Task<Result<TaskDto>> CreateTaskAsync(CreateTaskDto createTaskDto, Guid userId)
        {
            _logger.LogInformation("Creating new task with title {TaskTitle} in project {ProjectId}", createTaskDto.Title, createTaskDto.ProjectId);

            if (string.IsNullOrWhiteSpace(createTaskDto.Title))
                return Result<TaskDto>.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "Title"));

            var project = await _unitOfWork.Projects.GetByIdAsync(createTaskDto.ProjectId);
            if (project == null)
                return Result<TaskDto>.Failure(AppConstants.ErrorMessages.ProjectNotFound);

            if (createTaskDto.DueDate.HasValue && createTaskDto.StartDate.HasValue && createTaskDto.DueDate < createTaskDto.StartDate)
                return Result<TaskDto>.Failure(AppConstants.ValidationMessages.DueDateMustBeGreaterThanStartDate);

            var task = new Domain.Entities.Task
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                ProjectId = createTaskDto.ProjectId,
                Priority = (TaskPriority)createTaskDto.Priority,
                Status = Domain.Enums.TaskStatus.Todo,
                StartDate = createTaskDto.StartDate,
                DueDate = createTaskDto.DueDate,
                EstimatedHours = createTaskDto.EstimatedHours,
                ParentTaskId = createTaskDto.ParentTaskId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            var taskDto = _mapper.Map<TaskDto>(task);
            _logger.LogInformation("Task created successfully with id {TaskId}", task.Id);
            return Result<TaskDto>.Success(taskDto, string.Format(AppConstants.SuccessMessages.RecordCreated, "Task"));
        }

        public async Task<Result> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto)
        {
            _logger.LogInformation("Updating task with id {TaskId}", id);
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task with id {TaskId} not found for update", id);
                return Result.Failure(AppConstants.ErrorMessages.TaskNotFound);
            }

            if (string.IsNullOrWhiteSpace(updateTaskDto.Title))
                return Result.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "Title"));

            if (updateTaskDto.DueDate.HasValue && updateTaskDto.StartDate.HasValue && updateTaskDto.DueDate < updateTaskDto.StartDate)
                return Result.Failure(AppConstants.ValidationMessages.DueDateMustBeGreaterThanStartDate);

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.Priority = (TaskPriority)updateTaskDto.Priority;
            task.StartDate = updateTaskDto.StartDate;
            task.DueDate = updateTaskDto.DueDate;
            task.EstimatedHours = updateTaskDto.EstimatedHours;
            task.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with id {TaskId} updated successfully", id);
            return Result.Success(string.Format(AppConstants.SuccessMessages.RecordUpdated, "Task"));
        }

        public async Task<Result> DeleteTaskAsync(Guid id)
        {
            _logger.LogInformation("Deleting task with id {TaskId}", id);
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task with id {TaskId} not found for deletion", id);
                return Result.Failure(AppConstants.ErrorMessages.TaskNotFound);
            }

            _unitOfWork.Tasks.Remove(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with id {TaskId} deleted successfully", id);
            return Result.Success(string.Format(AppConstants.SuccessMessages.RecordDeleted, "Task"));
        }

        public async Task<Result> ChangeTaskStatusAsync(Guid id, int status)
        {
            _logger.LogInformation("Changing status of task {TaskId} to {Status}", id, status);
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task with id {TaskId} not found for status change", id);
                return Result.Failure("Task not found");
            }

            var oldStatus = task.Status.ToString();
            task.Status = (Domain.Enums.TaskStatus)status;
            task.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task {TaskId} status changed to {Status} successfully", id, status);

            if (task.AssignedToId.HasValue)
            {
                var assignedUser = await _unitOfWork.Users.GetByIdAsync(task.AssignedToId.Value);
                if (assignedUser != null)
                {
                    await _emailService.SendTaskStatusChangedEmailAsync(
                        assignedUser.Email,
                        $"{assignedUser.FirstName} {assignedUser.LastName}",
                        task.Title,
                        oldStatus,
                        task.Status.ToString()
                    );
                }
            }

            return Result.Success("Task status updated successfully");
        }

        public async Task<Result> AssignTaskAsync(Guid id, Guid userId)
        {
            _logger.LogInformation("Assigning task {TaskId} to user {UserId}", id, userId);
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task with id {TaskId} not found for assignment", id);
                return Result.Failure("Task not found");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found for task assignment", userId);
                return Result.Failure("User not found");
            }

            task.AssignedToId = userId;
            task.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task {TaskId} assigned to user {UserId} successfully", id, userId);

            await _emailService.SendTaskAssignedEmailAsync(user.Email, $"{user.FirstName} {user.LastName}", task.Title, "Task Management Project"
            );

            return Result.Success("Task assigned successfully");
        }

        public async Task<Result<PaginatedResponseDto<TaskDto>>> GetPagedTasksAsync(int pageNumber, int pageSize)
        {
            return await GetPagedAsync(
                (page, size) => _unitOfWork.Tasks.GetPagedAsync(page, size),
                pageNumber,
                pageSize,
                "Tasks"
            );
        }
    }
}
