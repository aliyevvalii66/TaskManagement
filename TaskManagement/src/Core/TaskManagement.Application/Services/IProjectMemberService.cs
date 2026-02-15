using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.ProjectMember;
using TaskManagement.Shared.Results;

namespace TaskManagement.Application.Services
{
    public interface IProjectMemberService
    {
        Task<Result<ProjectMemberDto>> GetMemberByIdAsync(Guid id);
        Task<Result<IEnumerable<ProjectMemberDto>>> GetProjectMembersAsync(Guid projectId);
        Task<Result<IEnumerable<ProjectMemberDto>>> GetUserProjectsAsync(Guid userId);
        Task<Result<ProjectMemberDto>> AddMemberAsync(AddProjectMemberDto addMemberDto);
        Task<Result> RemoveMemberAsync(Guid id);
        Task<Result> UpdateMemberAccessLevelAsync(Guid id, int accessLevel);
    }
}
