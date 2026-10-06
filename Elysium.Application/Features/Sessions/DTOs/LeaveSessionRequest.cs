using FluentValidation;

namespace Elysium.Application.Features.Sessions.DTOs;

public record LeaveSessionRequest(int studentId, int sessionId);


public class LeaveSessionRequestValidator : AbstractValidator<LeaveSessionRequest>
{
    public LeaveSessionRequestValidator()
    {

        RuleFor(x => x.sessionId)
            .GreaterThan(0)
            .WithMessage("Session id is not valid");

        RuleFor(x => x.studentId)
            .GreaterThan(0)
            .WithMessage("Student id is not valid");


    }
}
