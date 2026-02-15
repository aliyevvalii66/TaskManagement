using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.DTOs.User;
using TaskManagement.Shared.Results;

namespace TaskManagement.Application.Services
{
    public interface IProjectService
    {
        Task<Result<ProjectDto>> GetProjectByIdAsync(Guid id);
        Task<Result<PaginatedResponseDto<ProjectDto>>> GetPagedProjectsAsync(int pageNumber, int pageSize);
        Task<Result<IEnumerable<ProjectDto>>> GetAllProjectsAsync();
        Task<Result<IEnumerable<ProjectDto>>> GetUserProjectsAsync(Guid userId);
        Task<Result<ProjectDto>> CreateProjectAsync(CreateProjectDto createProjectDto, Guid userId);
        Task<Result> UpdateProjectAsync(Guid id, UpdateProjectDto updateProjectDto);
        Task<Result> DeleteProjectAsync(Guid id);
        Task<Result> ArchiveProjectAsync(Guid id);
    }
}
