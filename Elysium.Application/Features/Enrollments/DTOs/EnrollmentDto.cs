using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Enrollments.DTOs;

public record EnrollmentDto(int studentId, int courseId, DateTime EnrolledAt);

