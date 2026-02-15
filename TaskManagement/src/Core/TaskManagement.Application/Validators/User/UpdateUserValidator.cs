using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.User;

namespace TaskManagement.Application.Validators.User
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required")
                .MinimumLength(2).WithMessage("First Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("First Name must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required")
                .MinimumLength(2).WithMessage("Last Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Last Name must not exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone Number must not exceed 20 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Department)
                .MaximumLength(100).WithMessage("Department must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Department));
        }
    }

}
