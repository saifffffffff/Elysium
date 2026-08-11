using Elysium.Domain.Models;
using FluentValidation;

namespace Elysium.Application.Features.Users.DTOs;

public record CreateUserRequest(
    string username , 
    string password,
    string firstname,
    string lastname,
    DateOnly birthDate,
    UserRole role
);


public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        
        RuleFor(request => request.password)
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(request => request.username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(64)
            .WithMessage("Username cannot exceed 64 characters.");

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

        RuleFor(request => request.role)
            .NotNull()
            .WithMessage("Role is required.");

    }
}
