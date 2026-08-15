using Elysium.Application.Features.Courses.DTOs;
using Elysium.Application.Features.Teachers.Services;
using Elysium.Application.Features.Users.DTOs;
using Elysium.Application.Helpers;
using Elysium.Domain.Interfaces;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Elysium.Domain.Primitives;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Courses.Services;

public class CourseService( ITeacherRepository teacherRepository, IStudentRepository studentRepository, IEnrollmentRepository enrollmentRepository, IUnitOfWork unitOfWork, ICodeGenerator codeGenerator, IValidator<CreateCourseRequest> courseValidator , ICourseRepository courseRepository) : ICourseService
{
    public async Task<Result<bool>> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {

        if (string.IsNullOrEmpty(code))
            return Result<bool>.Failure("invalid code");

        bool exists = await courseRepository.ExistsAsync(c => c.Code == code , cancellationToken);

        
        return exists;

    }
    
    public async Task<Result<CreateCourseResponse>> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await courseValidator.ValidateAsync(request , cancellationToken);
        
        if ( !validationResult.IsValid )
            return Result<CreateCourseResponse>.Failure(validationResult.Errors.Select( error => new Error(error.ErrorMessage)).ToList());

        // business rule 1 - teacher exists
        var teacher = await teacherRepository.GetByIdAsync(request.teacherId , cancellationToken);

        if (teacher is null)
            return Result<CreateCourseResponse>.Failure($"Teacher with id {request.teacherId} does not exist.");

        // business rule 2 - code is unique

        string code;
        do
        {
            code = codeGenerator.GenerateRandomCode();
        }
        while ((await ExistsByCodeAsync(code, cancellationToken)).Value);



        var domainValidation = Course.Create(request.name, request.description, code, request.teacherId);

        if (!domainValidation.IsSuccess)
            return Result<CreateCourseResponse>.Failure(domainValidation.Errors);

        var course = domainValidation.Value!;
        await courseRepository.AddAsync(course, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCourseResponse(course.Id, course.Code);
        



    }

    

    private CourseDto ToDto(Course course) => new CourseDto(course.Id, course.Name, course.Description, course.Code  ,course.TeacherId, course.CreatedAt);

    public async Task<Result<IReadOnlyList<CourseDto>>> GetAllByTeacherId(int teacherId, CancellationToken cancellationToken = default)
    {
        
        bool exists= await teacherRepository.ExistsAsync(teacher => teacher.Id == teacherId, cancellationToken);
        
        if ( !exists )
            return Result<IReadOnlyList<CourseDto>>.Failure($"Teacher with id {teacherId} does not exist");

        var courses = await courseRepository.FindAsync(course => course.TeacherId == teacherId, cancellationToken);

        return courses.Select(ToDto).ToList();
    }

    public async Task<Result<IReadOnlyList<CourseDto>>> GetAllByStudentId(int studentId, CancellationToken cancellationToken = default)
    {
        bool exists = await studentRepository.ExistsAsync(student => student.Id == studentId, cancellationToken);

        if (!exists)
            return Result<IReadOnlyList<CourseDto>>.Failure($"Student with id {studentId} does not exist");

        var enrollments = await enrollmentRepository.GetAllByStudentAsync(studentId, cancellationToken);

        return enrollments.Select(enrollment => ToDto(enrollment.Course)).ToList();
    }

    Task<Result<IReadOnlyList<CourseDto>>> ICourseService.GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
