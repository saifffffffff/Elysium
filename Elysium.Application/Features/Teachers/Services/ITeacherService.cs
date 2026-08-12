using Elysium.Application.Features.Teachers.DTOs;
using Elysium.Domain.Primitives;

namespace Elysium.Application.Features.Teachers.Services;

public interface ITeacherService
{
    Task<Result<TeacherDto>> GetByIdAsync(int teacherId, CancellationToken ct = default);
    Task<Result<IEnumerable<TeacherDto>>> GetAllAsync(CancellationToken ct = default);
}