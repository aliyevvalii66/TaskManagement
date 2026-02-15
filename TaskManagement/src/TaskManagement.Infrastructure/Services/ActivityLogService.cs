using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.DTOs.ActivityLog;
using TaskManagement.Application.Repositories;
using TaskManagement.Application.Services;
using TaskManagement.Shared.Constants;
using TaskManagement.Shared.Results;
using DomainActivityLog = TaskManagement.Domain.Entities.ActivityLog;
namespace TaskManagement.Infrastructure.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ActivityLogService> _logger;

        public ActivityLogService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ActivityLogService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ActivityLogDto>> GetActivityByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting activity log with id {ActivityId}", id);
            var activity = await _unitOfWork.ActivityLogs.GetByIdAsync(id);

            if (activity == null)
            {
                _logger.LogWarning("Activity log with id {ActivityId} not found", id);
                return Result<ActivityLogDto>.Failure("Activity log not found");
            }

            var activityDto = _mapper.Map<ActivityLogDto>(activity);
            _logger.LogInformation("Activity log with id {ActivityId} retrieved successfully", id);
            return Result<ActivityLogDto>.Success(activityDto);
        }

        public async Task<Result<IEnumerable<ActivityLogDto>>> GetTaskActivityLogsAsync(Guid taskId)
        {
            _logger.LogInformation("Getting activity logs for task {TaskId}", taskId);
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);

            if (task == null)
            {
                _logger.LogWarning("Task with id {TaskId} not found", taskId);
                return Result<IEnumerable<ActivityLogDto>>.Failure(AppConstants.ErrorMessages.TaskNotFound);
            }

            var activityLogs = await _unitOfWork.ActivityLogs.FindAsync(a => a.TaskId == taskId);
            var activityDtos = _mapper.Map<List<ActivityLogDto>>(activityLogs.OrderByDescending(a => a.ActionAt));

            _logger.LogInformation("Retrieved {Count} activity logs for task {TaskId}", activityDtos.Count, taskId);
            return Result<IEnumerable<ActivityLogDto>>.Success(activityDtos);
        }

        public async Task<Result<IEnumerable<ActivityLogDto>>> GetProjectActivityLogsAsync(Guid projectId)
        {
            _logger.LogInformation("Getting activity logs for project {ProjectId}", projectId);
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);

            if (project == null)
            {
                _logger.LogWarning("Project with id {ProjectId} not found", projectId);
                return Result<IEnumerable<ActivityLogDto>>.Failure(AppConstants.ErrorMessages.ProjectNotFound);
            }

            var activityLogs = await _unitOfWork.ActivityLogs.FindAsync(a => a.ProjectId == projectId);
            var activityDtos = _mapper.Map<List<ActivityLogDto>>(activityLogs.OrderByDescending(a => a.ActionAt));

            _logger.LogInformation("Retrieved {Count} activity logs for project {ProjectId}", activityDtos.Count, projectId);
            return Result<IEnumerable<ActivityLogDto>>.Success(activityDtos);
        }

        public async Task<Result<IEnumerable<ActivityLogDto>>> GetUserActivityLogsAsync(Guid userId)
        {
            _logger.LogInformation("Getting activity logs for user {UserId}", userId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found", userId);
                return Result<IEnumerable<ActivityLogDto>>.Failure(AppConstants.ErrorMessages.UserNotFound);
            }

            var activityLogs = await _unitOfWork.ActivityLogs.FindAsync(a => a.UserId == userId);
            var activityDtos = _mapper.Map<List<ActivityLogDto>>(activityLogs.OrderByDescending(a => a.ActionAt));

            _logger.LogInformation("Retrieved {Count} activity logs for user {UserId}", activityDtos.Count, userId);
            return Result<IEnumerable<ActivityLogDto>>.Success(activityDtos);
        }

        public async Task<Result<ActivityLogDto>> LogActivityAsync(CreateActivityLogDto createActivityLogDto)
        {
            _logger.LogInformation("Creating activity log - Action: {Action}, User: {UserId}", createActivityLogDto.Action, createActivityLogDto.UserId);

            if (string.IsNullOrWhiteSpace(createActivityLogDto.Action))
                return Result<ActivityLogDto>.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "Action"));

            var user = await _unitOfWork.Users.GetByIdAsync(createActivityLogDto.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found for activity log", createActivityLogDto.UserId);
                return Result<ActivityLogDto>.Failure(AppConstants.ErrorMessages.UserNotFound);
            }

            if (createActivityLogDto.TaskId.HasValue)
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(createActivityLogDto.TaskId.Value);
                if (task == null)
                {
                    _logger.LogWarning("Task with id {TaskId} not found for activity log", createActivityLogDto.TaskId);
                    return Result<ActivityLogDto>.Failure(AppConstants.ErrorMessages.TaskNotFound);
                }
            }

            if (createActivityLogDto.ProjectId.HasValue)
            {
                var project = await _unitOfWork.Projects.GetByIdAsync(createActivityLogDto.ProjectId.Value);
                if (project == null)
                {
                    _logger.LogWarning("Project with id {ProjectId} not found for activity log", createActivityLogDto.ProjectId);
                    return Result<ActivityLogDto>.Failure(AppConstants.ErrorMessages.ProjectNotFound);
                }
            }

            var activityLog = new DomainActivityLog
            {
                UserId = createActivityLogDto.UserId,
                TaskId = createActivityLogDto.TaskId,
                ProjectId = createActivityLogDto.ProjectId,
                Action = createActivityLogDto.Action,
                Description = createActivityLogDto.Description,
                OldValue = createActivityLogDto.OldValue,
                NewValue = createActivityLogDto.NewValue,
                ActionAt = DateTime.UtcNow,
                CreatedBy = createActivityLogDto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ActivityLogs.AddAsync(activityLog);
            await _unitOfWork.SaveChangesAsync();

            var activityDto = _mapper.Map<ActivityLogDto>(activityLog);
            _logger.LogInformation("Activity log created successfully with id {ActivityLogId}", activityLog.Id);
            return Result<ActivityLogDto>.Success(activityDto, "Activity logged successfully");
        }
    }
}
