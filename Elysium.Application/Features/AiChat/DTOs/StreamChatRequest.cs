using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Elysium.Application.Features.AiChat.DTOs;

public record StreamChatRequest(int studentSessionId, string question);

public class StreamChatRequestValidator : AbstractValidator<StreamChatRequest>
{
    public StreamChatRequestValidator()
    {
        RuleFor( x => x.studentSessionId )
            .GreaterThan(0)
            .WithMessage("Student Session id must be greater than 0.");

        RuleFor(x => x.question)
            .NotEmpty()
            .WithMessage("Question cannot be empty.")
            .MaximumLength(1000)
            .WithMessage("Question cannot exceed 1,000 characters.");

    }
}

