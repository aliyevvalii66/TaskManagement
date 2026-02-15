using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.Services;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        [HttpGet("api/tasks/paged")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _taskService.GetPagedTasksAsync(pageNumber, pageSize);
            return Ok(result);
        }
        [HttpGet("api/tasks/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var result = await _taskService.GetTaskByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("api/tasks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTasks()
        {
            var result = await _taskService.GetAllTasksAsync();
            return Ok(result);
        }

        [HttpGet("api/tasks/project/{projectId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProjectTasks(Guid projectId)
        {
            var result = await _taskService.GetProjectTasksAsync(projectId);
            return Ok(result);
        }

        [HttpGet("api/tasks/user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserTasks(Guid userId)
        {
            var result = await _taskService.GetUserTasksAsync(userId);
            return Ok(result);
        }


        [Authorize]
        [HttpPost("api/tasks/create")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var userId = User.GetUserId();
            var result = await _taskService.CreateTaskAsync(createTaskDto, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetTaskById), new { id = result.Data.Id }, result);
        }
        [Authorize]
        [HttpPut("api/tasks/update/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _taskService.UpdateTaskAsync(id, updateTaskDto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpDelete("api/tasks/delete/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var result = await _taskService.DeleteTaskAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpPut("api/tasks/change-status/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeTaskStatus(Guid id, [FromBody] ChangeTaskStatusDto changeStatusDto)
        {
            var result = await _taskService.ChangeTaskStatusAsync(id, changeStatusDto.Status);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpPut("api/tasks/assign/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignTask(Guid id, [FromBody] AssignTaskDto assignTaskDto)
        {
            var result = await _taskService.AssignTaskAsync(id, assignTaskDto.UserId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
