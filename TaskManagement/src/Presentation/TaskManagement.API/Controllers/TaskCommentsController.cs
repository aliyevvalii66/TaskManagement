using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.DTOs.TaskComment;
using TaskManagement.Application.Services;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    public class TaskCommentsController : ControllerBase
    {
        private readonly ITaskCommentService _taskCommentService;

        public TaskCommentsController(ITaskCommentService taskCommentService)
        {
            _taskCommentService = taskCommentService;
        }

        [HttpGet("api/comments/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            var result = await _taskCommentService.GetCommentByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("api/comments/task/{taskId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskComments(Guid taskId)
        {
            var result = await _taskCommentService.GetTaskCommentsAsync(taskId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("api/comments/add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddComment([FromBody] CreateTaskCommentDto createCommentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            var result = await _taskCommentService.AddCommentAsync(createCommentDto, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetCommentById), new { id = result.Data.Id }, result);
        }
        [Authorize]
        [HttpPut("api/comments/update/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateTaskCommentDto updateCommentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _taskCommentService.UpdateCommentAsync(id, updateCommentDto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpDelete("api/comments/delete/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var result = await _taskCommentService.DeleteCommentAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
