using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Domain.Interfaces;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Elysium.Domain.Primitives;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Elysium.Application.Features.Sessions.Services;

public class SessionService( ISessionRepository sessionRepository, ICourseRepository courseRepository, IUnitOfWork unitOfWork ,  IValidator<CreateSessionRequest> createSessionValidator) : ISessionService
{
    private SessionDto ToDto(Session session) => new SessionDto(session.Id, session.Name, session.Description, session.Status, session.StartedAt, session.FinishedAt);
    
    public async Task<Result<int>> CreateAsync(CreateSessionRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await createSessionValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.Errors.Select(e => new Error(e.ErrorMessage)).ToList();
        
        // business rule 1 : course exists
        
        bool courseExists = await courseRepository.ExistsAsync(course => course.Id == request.courseId);

        if (!courseExists)
            return $"Course with id {request.courseId} does not exist";

        var sessionCreationResult = Session.Create(request.name, request.description,  request.courseId);

        if (!sessionCreationResult.IsSuccess)
            return sessionCreationResult.Errors;

        var session = sessionCreationResult.Value!;

        session.Start();

        await sessionRepository.AddAsync(session, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);


        return session.Id;




    }

    public async Task<Result<IReadOnlyCollection<SessionDto>>> GetAllByCourseIdAsync (int courseId , CancellationToken cancellationToken = default)
    {


        var course = await courseRepository.GetByIdWithSessionsAsync(courseId , cancellationToken);
        
        if (course is null)
            return $"Course with id {courseId} does not exist";

        return course.Sessions.Select(ToDto).ToImmutableList();

    }



}
