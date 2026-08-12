using Elysium.Application.Features.Students.DTOs;
using Elysium.Domain.Primitives;

namespace Elysium.Application.Features.Students.Services;

public interface IStudentService
{
    Task<Result<StudentDto>> GetByIdAsync(int studentId, CancellationToken ct = default);
    Task<Result<IEnumerable<StudentDto>>> GetAllAsync(CancellationToken ct = default);
}