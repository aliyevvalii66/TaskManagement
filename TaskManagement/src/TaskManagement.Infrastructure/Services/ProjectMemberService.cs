using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.ProjectMember;
using TaskManagement.Application.Repositories;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Enums;
using TaskManagement.Shared.Constants;
using TaskManagement.Shared.Results;
using DomainProjectMember = TaskManagement.Domain.Entities.ProjectMember;
namespace TaskManagement.Infrastructure.Services
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjectMemberService> _logger;
        private readonly IEmailService _emailService;
        public ProjectMemberService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProjectMemberService> logger, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<Result<ProjectMemberDto>> GetMemberByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting project member with id {MemberId}", id);
            var member = await _unitOfWork.ProjectMembers.GetByIdAsync(id);

            if (member == null)
            {
                _logger.LogWarning("Project member with id {MemberId} not found", id);
                return Result<ProjectMemberDto>.Failure("Project member not found");
            }

            var memberDto = _mapper.Map<ProjectMemberDto>(member);
            _logger.LogInformation("Project member with id {MemberId} retrieved successfully", id);
            return Result<ProjectMemberDto>.Success(memberDto);
        }

        public async Task<Result<IEnumerable<ProjectMemberDto>>> GetProjectMembersAsync(Guid projectId)
        {
            _logger.LogInformation("Getting members for project {ProjectId}", projectId);
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);

            if (project == null)
            {
                _logger.LogWarning("Project with id {ProjectId} not found", projectId);
                return Result<IEnumerable<ProjectMemberDto>>.Failure(AppConstants.ErrorMessages.ProjectNotFound);
            }

            var members = await _unitOfWork.ProjectMembers.FindAsync(m => m.ProjectId == projectId);
            var memberDtos = _mapper.Map<List<ProjectMemberDto>>(members);

            _logger.LogInformation("Retrieved {Count} members for project {ProjectId}", memberDtos.Count, projectId);
            return Result<IEnumerable<ProjectMemberDto>>.Success(memberDtos);
        }

        public async Task<Result<IEnumerable<ProjectMemberDto>>> GetUserProjectsAsync(Guid userId)
        {
            _logger.LogInformation("Getting projects for user {UserId}", userId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found", userId);
                return Result<IEnumerable<ProjectMemberDto>>.Failure(AppConstants.ErrorMessages.UserNotFound);
            }

            var memberships = await _unitOfWork.ProjectMembers.FindAsync(m => m.UserId == userId);
            var memberDtos = _mapper.Map<List<ProjectMemberDto>>(memberships);

            _logger.LogInformation("Retrieved {Count} project memberships for user {UserId}", memberDtos.Count, userId);
            return Result<IEnumerable<ProjectMemberDto>>.Success(memberDtos);
        }

        public async Task<Result<ProjectMemberDto>> AddMemberAsync(AddProjectMemberDto addMemberDto)
        {
            _logger.LogInformation("Adding user {UserId} to project {ProjectId}", addMemberDto.UserId, addMemberDto.ProjectId);

            var project = await _unitOfWork.Projects.GetByIdAsync(addMemberDto.ProjectId);
            if (project == null)
            {
                _logger.LogWarning("Project with id {ProjectId} not found", addMemberDto.ProjectId);
                return Result<ProjectMemberDto>.Failure("Project not found");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(addMemberDto.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found", addMemberDto.UserId);
                return Result<ProjectMemberDto>.Failure("User not found");
            }

            var existingMember = await _unitOfWork.ProjectMembers.FirstOrDefaultAsync(
                m => m.ProjectId == addMemberDto.ProjectId && m.UserId == addMemberDto.UserId);

            if (existingMember != null)
            {
                _logger.LogWarning("User {UserId} is already a member of project {ProjectId}", addMemberDto.UserId, addMemberDto.ProjectId);
                return Result<ProjectMemberDto>.Failure("User is already a member of this project");
            }

            var member = new DomainProjectMember
            {
                ProjectId = addMemberDto.ProjectId,
                UserId = addMemberDto.UserId,
                AccessLevel = (ProjectAccessLevel)addMemberDto.AccessLevel,
                JoinedAt = DateTime.UtcNow,
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ProjectMembers.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            var memberDto = _mapper.Map<ProjectMemberDto>(member);
            _logger.LogInformation("User {UserId} added to project {ProjectId} successfully", addMemberDto.UserId, addMemberDto.ProjectId);

            var projectOwner = await _unitOfWork.Users.GetByIdAsync(project.OwnerId);
            if (projectOwner != null)
            {
                await _emailService.SendProjectInvitationEmailAsync(
                    user.Email,
                    $"{user.FirstName} {user.LastName}",
                    project.Name,
                    $"{projectOwner.FirstName} {projectOwner.LastName}"
                );
            }

            return Result<ProjectMemberDto>.Success(memberDto, "Member added to project successfully");
        }

        public async Task<Result> RemoveMemberAsync(Guid id)
        {
            _logger.LogInformation("Removing project member with id {MemberId}", id);
            var member = await _unitOfWork.ProjectMembers.GetByIdAsync(id);

            if (member == null)
            {
                _logger.LogWarning("Project member with id {MemberId} not found for removal", id);
                return Result.Failure("Project member not found");
            }

            _unitOfWork.ProjectMembers.Remove(member);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project member with id {MemberId} removed successfully", id);
            return Result.Success("Member removed from project successfully");
        }

        public async Task<Result> UpdateMemberAccessLevelAsync(Guid id, int accessLevel)
        {
            _logger.LogInformation("Updating access level for project member {MemberId} to {AccessLevel}", id, accessLevel);
            var member = await _unitOfWork.ProjectMembers.GetByIdAsync(id);

            if (member == null)
            {
                _logger.LogWarning("Project member with id {MemberId} not found for access level update", id);
                return Result.Failure("Project member not found");
            }

            member.AccessLevel = (ProjectAccessLevel)accessLevel;
            member.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ProjectMembers.Update(member);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Access level for project member {MemberId} updated to {AccessLevel} successfully", id, accessLevel);
            return Result.Success("Member access level updated successfully");
        }
    }
}
