using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.ProjectMember;

namespace TaskManagement.Application.Validators.ProjectMember
{
    public class AddProjectMemberValidator : AbstractValidator<AddProjectMemberDto>
    {
        public AddProjectMemberValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.AccessLevel)
                .InclusiveBetween(0, 3).WithMessage("Invalid Access Level value");
        }
    }
}
