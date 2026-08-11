using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Users.DTOs;

public record UpdateProfileRequest(int id , string firstname , string lastname , DateOnly birthDate);



public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(request => request.firstname)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(64)
            .WithMessage("First name cannot exceed 64 characters.");

        RuleFor(request => request.lastname)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(64)
            .WithMessage("Last name cannot exceed 64 characters.");

        RuleFor(request => request.birthDate)
            .Must(birthDate => birthDate < DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Birth date must be in the past.");

        
    }
}

