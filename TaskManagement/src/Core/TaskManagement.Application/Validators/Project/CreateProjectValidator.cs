using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.Project;

namespace TaskManagement.Application.Validators.Project
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project Name is required")
                .MinimumLength(3).WithMessage("Project Name must be at least 3 characters")
                .MaximumLength(255).WithMessage("Project Name must not exceed 255 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start Date is required")
                .GreaterThan(DateTime.MinValue).WithMessage("Start Date must be valid");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End Date must be greater than Start Date")
                .When(x => x.EndDate.HasValue);
        }
    }
}
