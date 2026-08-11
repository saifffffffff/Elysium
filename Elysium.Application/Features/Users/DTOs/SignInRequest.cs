using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Users.DTOs;

public record SignInRequest(string username, string password);


public class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(request => request.username)
            .NotEmpty()
            .WithMessage("Username is required");

        RuleFor(request => request.password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}

