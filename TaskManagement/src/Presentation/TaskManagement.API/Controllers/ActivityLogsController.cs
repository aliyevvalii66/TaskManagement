using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.ActivityLog;
using TaskManagement.Application.Services;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    public class ActivityLogsController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogsController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        [HttpGet("api/activity-logs/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActivityById(Guid id)
        {
            var result = await _activityLogService.GetActivityByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("api/activity-logs/task/{taskId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskActivityLogs(Guid taskId)
        {
            var result = await _activityLogService.GetTaskActivityLogsAsync(taskId);
            return Ok(result);
        }

        [HttpGet("api/activity-logs/project/{projectId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProjectActivityLogs(Guid projectId)
        {
            var result = await _activityLogService.GetProjectActivityLogsAsync(projectId);
            return Ok(result);
        }

        [HttpGet("api/activity-logs/user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserActivityLogs(Guid userId)
        {
            var result = await _activityLogService.GetUserActivityLogsAsync(userId);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("api/activity-logs/log")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LogActivity([FromBody] CreateActivityLogDto createActivityLogDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _activityLogService.LogActivityAsync(createActivityLogDto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetActivityById), new { id = result.Data.Id }, result);
        }
    }
}
