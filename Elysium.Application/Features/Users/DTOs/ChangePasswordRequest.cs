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
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[^A-Za-z0-9]").WithMessage("Password must contain at least one special character.")
            .MaximumLength(128);
    }
}
