using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Elysium.Application.Features.Sessions.DTOs;

public enum Role { Teacher , Student}
public record JoinSessionRequest( int studentId , int sessionId );


public class JoinSessionRequestValidator : AbstractValidator<JoinSessionRequest>
{
    public JoinSessionRequestValidator()
    {

        RuleFor(x => x.sessionId)
            .GreaterThan(0)
            .WithMessage("Session id is not valid");

        RuleFor(x => x.studentId)
            .GreaterThan(0)
            .WithMessage("Student id is not valid");
       
        
    }
}

