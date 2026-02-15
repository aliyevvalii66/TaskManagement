using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.User;

namespace TaskManagement.Application.Validators.User
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required")
                .MinimumLength(2).WithMessage("First Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("First Name must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required")
                .MinimumLength(2).WithMessage("Last Name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Last Name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone Number must not exceed 20 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Department)
                .MaximumLength(100).WithMessage("Department must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Department));
        }
    }
}
