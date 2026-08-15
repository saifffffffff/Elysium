using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Enrollments.DTOs;

public record EnrollStudentRequest(int studentId, string courseCode);
