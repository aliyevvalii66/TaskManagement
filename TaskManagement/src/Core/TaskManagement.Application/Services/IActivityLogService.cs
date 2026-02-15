using TaskManagement.Application.DTOs.ActivityLog;
using TaskManagement.Shared.Results;

namespace TaskManagement.Application.Services
{
    public interface IActivityLogService
    {
        Task<Result<ActivityLogDto>> GetActivityByIdAsync(Guid id);
        Task<Result<IEnumerable<ActivityLogDto>>> GetTaskActivityLogsAsync(Guid taskId);
        Task<Result<IEnumerable<ActivityLogDto>>> GetProjectActivityLogsAsync(Guid projectId);
        Task<Result<IEnumerable<ActivityLogDto>>> GetUserActivityLogsAsync(Guid userId);
        Task<Result<ActivityLogDto>> LogActivityAsync(CreateActivityLogDto createActivityLogDto);
    }
}
