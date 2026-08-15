using Elysium.Application.Features.Enrollments.DTOs;
using Elysium.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Enrollments.Services;

public interface IEnrollmentService
{
    public Task<Result<EnrollmentDto>> EnrollStudentIntoCourse(EnrollStudentRequest request , CancellationToken cancellationToken = default);
}
