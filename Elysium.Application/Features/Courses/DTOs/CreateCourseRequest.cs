using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Courses.DTOs;

public record CreateCourseRequest(string name, string? description , int teacherId);

public class CreateCourseValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseValidator()
    {
        // TODO add messages 
        RuleFor(request => request.name)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(request => request.description)
            .Must(request => request is null || request.Length <= 512);

        RuleFor(request => request.teacherId)
            .GreaterThan(0);
    }
}

