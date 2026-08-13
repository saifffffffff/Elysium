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
        // TODO : add a password validation in the domain model and the database
        RuleFor(request => request.password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[^A-Za-z0-9]").WithMessage("Password must contain at least one special character.")
            .MaximumLength(128);

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
