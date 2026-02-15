using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.Services;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        [HttpGet("api/projects/paged")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedProjects([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _projectService.GetPagedProjectsAsync(pageNumber, pageSize);
            return Ok(result);
        }
        [HttpGet("api/projects/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var result = await _projectService.GetProjectByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("api/projects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProjects()
        {
            var result = await _projectService.GetAllProjectsAsync();
            return Ok(result);
        }

        [HttpGet("api/projects/user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserProjects(Guid userId)
        {
            var result = await _projectService.GetUserProjectsAsync(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("api/projects/create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createProjectDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            var result = await _projectService.CreateProjectAsync(createProjectDto, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetProjectById), new { id = result.Data.Id }, result);
        }
        [Authorize]
        [HttpPut("api/projects/update/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto updateProjectDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectService.UpdateProjectAsync(id, updateProjectDto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpDelete("api/projects/delete/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var result = await _projectService.DeleteProjectAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpPut("api/projects/archive/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ArchiveProject(Guid id)
        {
            var result = await _projectService.ArchiveProjectAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
