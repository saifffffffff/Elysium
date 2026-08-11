using Elysium.Infrastructure.Context;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Elysium.Infrastructure.Repositories;

public class EnrollmentRepository(AppDbContext context) : Repository<Enrollment>(context), IEnrollmentRepository
{
    public async Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default)
    {
        return await context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);
    }
}
