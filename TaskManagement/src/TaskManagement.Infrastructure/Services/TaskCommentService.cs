using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.TaskComment;
using TaskManagement.Application.Repositories;
using TaskManagement.Application.Services;
using TaskManagement.Shared.Constants;
using TaskManagement.Shared.Results;
using DomainTaskComment = TaskManagement.Domain.Entities.TaskComment;
namespace TaskManagement.Infrastructure.Services
{
    public class TaskCommentService : ITaskCommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskCommentService> _logger;
        private readonly IEmailService _emailService;

        public TaskCommentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TaskCommentService> logger, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<Result<TaskCommentDto>> GetCommentByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting comment with id {CommentId}", id);
            var comment = await _unitOfWork.TaskComments.GetByIdAsync(id);

            if (comment == null)
            {
                _logger.LogWarning("Comment with id {CommentId} not found", id);
                return Result<TaskCommentDto>.Failure("Comment not found");
            }

            var commentDto = _mapper.Map<TaskCommentDto>(comment);
            _logger.LogInformation("Comment with id {CommentId} retrieved successfully", id);
            return Result<TaskCommentDto>.Success(commentDto);
        }

        public async Task<Result<IEnumerable<TaskCommentDto>>> GetTaskCommentsAsync(Guid taskId)
        {
            _logger.LogInformation("Getting comments for task {TaskId}", taskId);
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);

            if (task == null)
            {
                _logger.LogWarning("Task with id {TaskId} not found", taskId);
                return Result<IEnumerable<TaskCommentDto>>.Failure(AppConstants.ErrorMessages.TaskNotFound);
            }

            var comments = await _unitOfWork.TaskComments.FindAsync(c => c.TaskId == taskId);
            var commentDtos = _mapper.Map<List<TaskCommentDto>>(comments);

            _logger.LogInformation("Retrieved {Count} comments for task {TaskId}", commentDtos.Count, taskId);
            return Result<IEnumerable<TaskCommentDto>>.Success(commentDtos);
        }

        public async Task<Result<TaskCommentDto>> AddCommentAsync(CreateTaskCommentDto createCommentDto, Guid userId)
        {
            _logger.LogInformation("Adding comment to task {TaskId} by user {UserId}", createCommentDto.TaskId, userId);

            if (string.IsNullOrWhiteSpace(createCommentDto.Content))
                return Result<TaskCommentDto>.Failure("Comment Content is required");

            var task = await _unitOfWork.Tasks.GetByIdAsync(createCommentDto.TaskId);
            if (task == null)
            {
                _logger.LogWarning("Task with id {TaskId} not found for comment", createCommentDto.TaskId);
                return Result<TaskCommentDto>.Failure("Task not found");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found for comment", userId);
                return Result<TaskCommentDto>.Failure("User not found");
            }

            var comment = new DomainTaskComment
            {
                TaskId = createCommentDto.TaskId,
                UserId = userId,
                Content = createCommentDto.Content,
                CommentedAt = DateTime.UtcNow,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TaskComments.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            var commentDto = _mapper.Map<TaskCommentDto>(comment);
            _logger.LogInformation("Comment added successfully with id {CommentId} to task {TaskId}", comment.Id, createCommentDto.TaskId);

            if (task.AssignedToId.HasValue)
            {
                var assignedUser = await _unitOfWork.Users.GetByIdAsync(task.AssignedToId.Value);
                if (assignedUser != null)
                {
                    await _emailService.SendCommentMentionEmailAsync(
                        assignedUser.Email,
                        $"{assignedUser.FirstName} {assignedUser.LastName}",
                        task.Title,
                        $"{user.FirstName} {user.LastName}",
                        createCommentDto.Content
                    );
                }
            }

            return Result<TaskCommentDto>.Success(commentDto, "Comment added successfully");
        }

        public async Task<Result> UpdateCommentAsync(Guid id, UpdateTaskCommentDto updateCommentDto)
        {
            _logger.LogInformation("Updating comment with id {CommentId}", id);
            var comment = await _unitOfWork.TaskComments.GetByIdAsync(id);

            if (comment == null)
            {
                _logger.LogWarning("Comment with id {CommentId} not found for update", id);
                return Result.Failure("Comment not found");
            }

            if (string.IsNullOrWhiteSpace(updateCommentDto.Content))
                return Result.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "Comment Content"));

            comment.Content = updateCommentDto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TaskComments.Update(comment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Comment with id {CommentId} updated successfully", id);
            return Result.Success("Comment updated successfully");
        }

        public async Task<Result> DeleteCommentAsync(Guid id)
        {
            _logger.LogInformation("Deleting comment with id {CommentId}", id);
            var comment = await _unitOfWork.TaskComments.GetByIdAsync(id);

            if (comment == null)
            {
                _logger.LogWarning("Comment with id {CommentId} not found for deletion", id);
                return Result.Failure("Comment not found");
            }

            _unitOfWork.TaskComments.Remove(comment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Comment with id {CommentId} deleted successfully", id);
            return Result.Success("Comment deleted successfully");
        }
    }
}
