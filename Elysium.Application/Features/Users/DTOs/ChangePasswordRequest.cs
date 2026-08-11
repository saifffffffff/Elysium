using FluentValidation;

namespace Elysium.Application.Features.Users.DTOs;

public record ChangePasswordRequest(int id, string currentPassword, string newPassword);


public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(request => request.currentPassword)
            .NotEmpty()
            .WithMessage("Current password is required");

        RuleFor(request => request.newPassword)
            .NotEmpty()
            .WithMessage("New password is required")
            .MaximumLength(64)
            .WithMessage("New password cannot exceed 64 characters.");
    }
}
