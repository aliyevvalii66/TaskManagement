using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.TaskComment;

namespace TaskManagement.Application.Validators.TaskComment
{
    public class CreateTaskCommentValidator : AbstractValidator<CreateTaskCommentDto>
    {
        public CreateTaskCommentValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty().WithMessage("Task ID is required");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment Content is required")
                .MinimumLength(1).WithMessage("Comment must not be empty")
                .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters");
        }
    }
}
