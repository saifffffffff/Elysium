using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Courses.DTOs;

public record CourseDto(string name, string? description, string code , int teacherId , DateTime createdAt);

