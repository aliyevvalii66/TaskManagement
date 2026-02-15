using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.ProjectMember;
using TaskManagement.Application.Services;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    public class ProjectMembersController : ControllerBase
    {
        private readonly IProjectMemberService _projectMemberService;

        public ProjectMembersController(IProjectMemberService projectMemberService)
        {
            _projectMemberService = projectMemberService;
        }

        [HttpGet("api/project-members/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMemberById(Guid id)
        {
            var result = await _projectMemberService.GetMemberByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("api/project-members/project/{projectId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProjectMembers(Guid projectId)
        {
            var result = await _projectMemberService.GetProjectMembersAsync(projectId);
            return Ok(result);
        }

        [HttpGet("api/project-members/user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserProjects(Guid userId)
        {
            var result = await _projectMemberService.GetUserProjectsAsync(userId);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("api/project-members/add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMember([FromBody] AddProjectMemberDto addMemberDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectMemberService.AddMemberAsync(addMemberDto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetMemberById), new { id = result.Data.Id }, result);
        }
        [Authorize]
        [HttpDelete("api/project-members/remove/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveMember(Guid id)
        {
            var result = await _projectMemberService.RemoveMemberAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [Authorize]
        [HttpPut("api/project-members/update-access-level/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAccessLevel(Guid id, [FromBody] UpdateProjectMemberDto updateMemberDto)
        {
            var result = await _projectMemberService.UpdateMemberAccessLevelAsync(id, updateMemberDto.AccessLevel);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
