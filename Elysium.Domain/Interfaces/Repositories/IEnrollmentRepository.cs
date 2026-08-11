using Elysium.Domain.Models;
using System.Linq.Expressions;

namespace Elysium.Domain.Interfaces.Repositories;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    
}
