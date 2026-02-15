using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.Task;

namespace TaskManagement.Application.Validators.Task
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task Title is required")
                .MinimumLength(3).WithMessage("Task Title must be at least 3 characters")
                .MaximumLength(255).WithMessage("Task Title must not exceed 255 characters");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Priority)
                .InclusiveBetween(0, 3).WithMessage("Invalid Priority value");

            RuleFor(x => x.DueDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("Due Date must be greater than Start Date")
                .When(x => x.DueDate.HasValue && x.StartDate.HasValue);

            RuleFor(x => x.EstimatedHours)
                .GreaterThan(0).WithMessage("Estimated Hours must be greater than 0")
                .When(x => x.EstimatedHours.HasValue);
        }
    }
}
