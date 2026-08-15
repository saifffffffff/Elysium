using Elysium.Application.Features.Courses.DTOs;
using Elysium.Domain.Models;
using Elysium.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Courses.Services;

public interface ICourseService
{

    Task<Result<CreateCourseResponse>> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<CourseDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<CourseDto>>> GetAllByTeacherId(int teacherId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<CourseDto>>> GetAllByStudentId(int studentId, CancellationToken cancellationToken = default);

}
