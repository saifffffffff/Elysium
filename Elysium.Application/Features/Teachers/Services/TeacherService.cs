using Elysium.Application.Features.Teachers.DTOs;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Elysium.Domain.Primitives;

namespace Elysium.Application.Features.Teachers.Services;

public class TeacherService(ITeacherRepository teacherRepository) : ITeacherService
{
    public async Task<Result<TeacherDto>> GetByIdAsync(int teacherId, CancellationToken ct = default)
    {
        var teacher = await teacherRepository.GetByIdWithProfileAsync(teacherId, ct);

        if (teacher is null)
            return Result<TeacherDto>.Failure("Teacher not found");

        return Result<TeacherDto>.Success(ToDto(teacher));
    }

    public async Task<Result<IEnumerable<TeacherDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var teachers = await teacherRepository.GetAllWithProfileAsync(ct);

        return Result<IEnumerable<TeacherDto>>.Success(teachers.Select(ToDto));
    }

    private static TeacherDto ToDto(Teacher teacher) =>
        new(teacher.Id, teacher.User.Username, teacher.User.FirstName, teacher.User.LastName, teacher.User.BirthDate);
}