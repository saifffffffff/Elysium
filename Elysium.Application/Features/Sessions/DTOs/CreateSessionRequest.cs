using FluentValidation;

namespace Elysium.Application.Features.Sessions.DTOs;

public record CreateSessionRequest(string name, string? description, int courseId);

public class CreateSessionRequestValidator : AbstractValidator<CreateSessionRequest>
{
    public CreateSessionRequestValidator()
    {
        RuleFor(request => request.name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(128)
            .WithMessage("Name cannot exceed 128 characters.");

        RuleFor(request => request.description)
            .Must(description => description is null || description.Length <= 512)
            .WithMessage("Description cannot exceed 512 characters.");


        RuleFor(request => request.courseId)
            .GreaterThan(0)
            .WithMessage("Course id must be greater than 0.");
    }
}