using Elysium.Application.Features.Enrollments.DTOs;
using Elysium.Domain.Interfaces;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Elysium.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Enrollments.Services;

public class EnrollmentService(IStudentRepository studentRepositry , ICourseRepository courseRepository , IEnrollmentRepository enrollmentRepository , IUnitOfWork unitOfWork): IEnrollmentService
{
    private EnrollmentDto ToDto(Enrollment enrollment) => new EnrollmentDto(enrollment.StudentId, enrollment.CourseId, enrollment.EnrollmentDate);

    public async Task<Result<EnrollmentDto>> EnrollStudentIntoCourse(EnrollStudentRequest request , CancellationToken cancellationToke = default )
    {
        // business rule : 1 - student must exist
        var studentExists = await studentRepositry.ExistsAsync( student => student.Id == request.studentId , cancellationToke);
        
        if (!studentExists)
            return Result<EnrollmentDto>.Failure($"Student with id {request.studentId} does not exist");

        // business rule : 2 - course must exist
        var course = await courseRepository.GetByCodeAsync(request.courseCode , cancellationToke);

        if (course is null )
            return Result<EnrollmentDto>.Failure($"Course with code {request.courseCode} does not exist");

        // business rules : 3 - student must not be enrolled in this course before

        bool enrollmentExists = await enrollmentRepository.ExistsAsync(enrollment => enrollment.StudentId == request.studentId && enrollment.CourseId == course.Id , cancellationToke);
        
        if (enrollmentExists)
            return Result<EnrollmentDto>.Failure("Student already enrolled in this course");


        // business rules met - continue :

        var enrollment = Enrollment.Create(request.studentId, course.Id);

        await enrollmentRepository.AddAsync(enrollment , cancellationToke);
        await unitOfWork.SaveChangesAsync(cancellationToke);

        return Result<EnrollmentDto>.Success(ToDto(enrollment));

    }
}
