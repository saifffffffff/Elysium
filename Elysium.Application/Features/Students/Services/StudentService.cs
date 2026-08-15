using Elysium.Application.Features.Students.DTOs;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Elysium.Domain.Primitives;

namespace Elysium.Application.Features.Students.Services;

public class StudentService(IStudentRepository studentRepository) : IStudentService
{
    public async Task<Result<StudentDto>> GetByIdAsync(int studentId, CancellationToken ct = default)
    {
        var student = await studentRepository.GetByIdWithProfileAsync(studentId, ct);

        if (student is null)
            return Result<StudentDto>.Failure("Student not found");

        return Result<StudentDto>.Success(ToDto(student));
    }

    public async Task<Result<IEnumerable<StudentDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var students = await studentRepository.GetAllWithProfileAsync(ct);

        return Result<IEnumerable<StudentDto>>.Success(students.Select(ToDto));
    }

    private static StudentDto ToDto(Student student) =>
        new(student.Id, student.User.Username, student.User.FirstName, student.User.LastName, student.User.BirthDate);


}