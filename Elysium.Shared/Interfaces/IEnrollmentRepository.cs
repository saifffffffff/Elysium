using Elysium.Shared.Models;
using System.Linq.Expressions;

namespace Elysium.Shared.Interfaces;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    
}
