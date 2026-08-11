using FluentValidation;

namespace Elysium.Application.Features.Users.DTOs;

public record ChangeUsernameRequest(int id, string username);


public class ChangeUsernameRequestValidator : AbstractValidator<ChangeUsernameRequest>
{
    public ChangeUsernameRequestValidator()
    {
        RuleFor(request => request.username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MaximumLength(64)
            .WithMessage("Username cannot exceed 64 characters.");
    }
}
