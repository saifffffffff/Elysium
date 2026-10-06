using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Application.Features.Transcription.DTOs;
using Elysium.Domain.Interfaces;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Elysium.Domain.Primitives;
using FluentValidation;

namespace Elysium.Application.Features.Sessions.Services;

public class SessionService( ISessionRepository sessionRepository, ICourseRepository courseRepository,IStudentRepository studentRepository , ITeacherRepository teacherRepository, IEnrollmentRepository enrollmentRepository, IStudentSessionRepository studentSessionRepository ,IUnitOfWork unitOfWork , ISessionNotifier sessionNotifier,  IValidator<StartSessionRequest> createSessionValidator , IValidator<JoinSessionRequest> joinSessionValidator, IValidator<LeaveSessionRequest> leaveSessionValidator) : ISessionService
{

    private SessionDto ToDto(Session session) => new SessionDto(session.Id, session.Name, session.Description, session.Status, session.StartedAt, session.FinishedAt);
    
    public async Task<Result<IReadOnlyCollection<SessionDto>>> GetAllByCourseIdAsync (int courseId , CancellationToken cancellationToken = default)
    {

        var course = await courseRepository.GetByIdWithSessionsAsync(courseId , cancellationToken);
        
        if (course is null)
            return $"Course with id {courseId} does not exist";

        return course.Sessions.Select(ToDto).ToImmutableList();

    }

    public async Task<Result> EndAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        var session = await sessionRepository.GetByIdAsync(sessionId,  cancellationToken);

        if (session is null)
            return $"Session with id {sessionId} does not exist";

        if (session.IsFinished)
            return $"Session with id {sessionId} already finished";

        session.Finish();

        sessionRepository.Update(session);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await sessionNotifier.NotifySessionEndedAsync(session.CourseId, sessionId, cancellationToken);

        return Result.Success();

    }

    public async Task<Result<JoinSessionResponse>> JoinAsync(JoinSessionRequest request , CancellationToken cancellationToken = default )
    {

        var validationResult = joinSessionValidator.Validate(request);

        if (!validationResult.IsValid)
            return validationResult.Errors.Select(error => new Error(error.ErrorMessage)).ToList();

        // business rules 
        // student exists - session exists - student enrolled in this course - session is active - student is not already in session - does student joined the session previuosly

        var studentExists = await studentRepository.ExistsAsync(s => s.Id == request.studentId);

        if (!studentExists)
            return $"student with id {request.studentId} does not exist";

        var session = await sessionRepository.GetByIdWithTranscriptAsync(request.sessionId);

        if (session is null)
            return $"session with id {request.sessionId} does not exist";

        var isEnrolled = await enrollmentRepository.IsStudentEnrolled(request.studentId, session.CourseId, cancellationToken);

        if (!isEnrolled)
            return $"student is not enrolled in session's course";

        if (session.IsFinished)
            return $"session is already finished";

        var isAlreadyInSession = await studentSessionRepository.IsStudentInSession(request.studentId, cancellationToken);
        
        if (isAlreadyInSession)
            return "student is already in session";

        
        var joinedBefore = await studentSessionRepository.GetByStudentIdAndSessionId(request.sessionId , request.studentId);

        int studentsessionId;

        if ( joinedBefore is not null )
        {
            joinedBefore.Rejoin();
            
            studentsessionId = joinedBefore.Id;
            
            await unitOfWork.SaveChangesAsync();
        }

        else
        {
            var sessionStudentCreationResult = StudentSession.Create(request.studentId, request.sessionId);

            if (!sessionStudentCreationResult.IsSuccess)
                return sessionStudentCreationResult.Errors;

            var studentSession = sessionStudentCreationResult.Value;
        
            await studentSessionRepository.AddAsync(studentSession!, cancellationToken);
            
            await unitOfWork.SaveChangesAsync();
            
            studentsessionId = studentSession!.Id;

        }

        Console.WriteLine( studentsessionId);
       
        return new JoinSessionResponse(studentsessionId, session.TranscriptSegments.Select(t => new TranscriptionSegmentDto(t.Text, t.StartTime, t.EndTime))); 

            
        
    }

    public async Task<Result> LeaveAsync(LeaveSessionRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = leaveSessionValidator.Validate(request);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.Select(error => new Error(error.ErrorMessage)).ToList());

        var studentSession = await studentSessionRepository.GetByStudentIdAndSessionId(request.sessionId, request.studentId, cancellationToken);

        if (studentSession is null)
            return Result.Failure("student has not joined this session");

        if (studentSession.HasLeft)
            return Result.Failure("student has already left this session");

        studentSession.Leave();

        studentSessionRepository.Update(studentSession);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        
        return Result.Success();
    }
    
    public async Task<Result<int>> StartAsync(StartSessionRequest request , CancellationToken cancellationToken = default )
    {

        var validationResult = await createSessionValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.Errors.Select(e => new Error(e.ErrorMessage)).ToList();


        bool courseExists = await courseRepository.ExistsAsync(course => course.Id == request.courseId);

        if (!courseExists)
            return $"Course with id {request.courseId} does not exist";
        
        var teacherExists = await teacherRepository.ExistsAsync(s => s.Id == request.teacherId);

        if (!teacherExists)
            return $"teacher with id {request.teacherId} does not exist";

        var isAssignedToCourse = await courseRepository.IsTeacherAssignedToCourse(request.teacherId, request.courseId, cancellationToken);

        if (!isAssignedToCourse)
            return $"teacher is not assigned to this course";

        var sessionCreationResult = Session.Create(request.name, request.description, request.courseId);

        if (!sessionCreationResult.IsSuccess)
            return sessionCreationResult.Errors;

        var session = sessionCreationResult.Value!;

        session.Start();

        await sessionRepository.AddAsync(session, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await sessionNotifier.NotifySessionCreatedAsync(session.CourseId, ToDto(session), cancellationToken);

        return session.Id;



    }
    
    public async Task DeleteByIdAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        await sessionRepository.DeleteByIdAsync(sessionId);
    }
}
