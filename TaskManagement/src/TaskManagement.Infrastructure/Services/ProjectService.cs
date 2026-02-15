using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.DTOs.User;
using TaskManagement.Application.Repositories;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Shared.Constants;
using TaskManagement.Shared.Results;

namespace TaskManagement.Infrastructure.Services
{
    public class ProjectService : BaseService<Project, ProjectDto>, IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProjectService> logger) : base(mapper , logger)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProjectDto>> GetProjectByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting project with id {ProjectId}", id);
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null)
            {
                _logger.LogWarning("Project with id {ProjectId} not found", id);
                return Result<ProjectDto>.Failure(AppConstants.ErrorMessages.ProjectNotFound);
            }

            var projectDto = _mapper.Map<ProjectDto>(project);
            _logger.LogInformation("Project with id {ProjectId} retrieved successfully", id);
            return Result<ProjectDto>.Success(projectDto);
        }

        public async Task<Result<IEnumerable<ProjectDto>>> GetAllProjectsAsync()
        {
            _logger.LogInformation("Getting all projects");
            var projects = await _unitOfWork.Projects.GetAllAsync();
            var projectDtos = _mapper.Map<List<ProjectDto>>(projects);

            _logger.LogInformation("Retrieved {Count} projects", projectDtos.Count);
            return Result<IEnumerable<ProjectDto>>.Success(projectDtos);
        }

        public async Task<Result<IEnumerable<ProjectDto>>> GetUserProjectsAsync(Guid userId)
        {
            _logger.LogInformation("Getting projects for user {UserId}", userId);
            var projects = await _unitOfWork.Projects.FindAsync(p => p.OwnerId == userId);
            var projectDtos = _mapper.Map<List<ProjectDto>>(projects);

            _logger.LogInformation("Retrieved {Count} projects for user {UserId}", projectDtos.Count, userId);
            return Result<IEnumerable<ProjectDto>>.Success(projectDtos);
        }

        public async Task<Result<ProjectDto>> CreateProjectAsync(CreateProjectDto createProjectDto, Guid userId)
        {
            _logger.LogInformation("Creating new project with name {ProjectName} by user {UserId}", createProjectDto.Name, userId);

            if (string.IsNullOrWhiteSpace(createProjectDto.Name))
                return Result<ProjectDto>.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "Project Name"));

            if (createProjectDto.StartDate == default)
                return Result<ProjectDto>.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "Start Date"));

            if (createProjectDto.EndDate.HasValue && createProjectDto.EndDate < createProjectDto.StartDate)
                return Result<ProjectDto>.Failure(AppConstants.ValidationMessages.DueDateMustBeGreaterThanStartDate);

            var project = new Domain.Entities.Project
            {
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                OwnerId = userId,
                StartDate = createProjectDto.StartDate,
                EndDate = createProjectDto.EndDate,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.SaveChangesAsync();

            var projectDto = _mapper.Map<ProjectDto>(project);
            _logger.LogInformation("Project created successfully with id {ProjectId}", project.Id);
            return Result<ProjectDto>.Success(projectDto, string.Format(AppConstants.SuccessMessages.RecordCreated, "Project"));
        }

        public async Task<Result> UpdateProjectAsync(Guid id, UpdateProjectDto updateProjectDto)
        {
            _logger.LogInformation("Updating project with id {ProjectId}", id);
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null)
            {
                _logger.LogWarning("Project with id {ProjectId} not found for update", id);
                return Result.Failure(AppConstants.ErrorMessages.ProjectNotFound);
            }

            if (string.IsNullOrWhiteSpace(updateProjectDto.Name))
                return Result.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "Project Name"));

            if (updateProjectDto.EndDate.HasValue && updateProjectDto.EndDate < updateProjectDto.StartDate)
                return Result.Failure(AppConstants.ValidationMessages.DueDateMustBeGreaterThanStartDate);

            project.Name = updateProjectDto.Name;
            project.Description = updateProjectDto.Description;
            project.StartDate = updateProjectDto.StartDate;
            project.EndDate = updateProjectDto.EndDate;
            project.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project with id {ProjectId} updated successfully", id);
            return Result.Success(string.Format(AppConstants.SuccessMessages.RecordUpdated, "Project"));
        }

        public async Task<Result> DeleteProjectAsync(Guid id)
        {
            _logger.LogInformation("Deleting project with id {ProjectId}", id);
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null)
            {
                _logger.LogWarning("Project with id {ProjectId} not found for deletion", id);
                return Result.Failure(AppConstants.ErrorMessages.ProjectNotFound);
            }

            _unitOfWork.Projects.Remove(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project with id {ProjectId} deleted successfully", id);
            return Result.Success(string.Format(AppConstants.SuccessMessages.RecordDeleted, "Project"));
        }

        public async Task<Result> ArchiveProjectAsync(Guid id)
        {
            _logger.LogInformation("Archiving project with id {ProjectId}", id);
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null)
            {
                _logger.LogWarning("Project with id {ProjectId} not found for archiving", id);
                return Result.Failure(AppConstants.ErrorMessages.ProjectNotFound);
            }

            project.IsArchived = true;
            project.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project with id {ProjectId} archived successfully", id);
            return Result.Success(string.Format(AppConstants.SuccessMessages.RecordUpdated, "Project"));
        }

        public async Task<Result<PaginatedResponseDto<ProjectDto>>> GetPagedProjectsAsync(int pageNumber, int pageSize)
        {
            return await GetPagedAsync(
                (page, size) => _unitOfWork.Projects.GetPagedAsync(page, size),
                pageNumber,
                pageSize,
                "Projects"
            );
        }
    }
}
