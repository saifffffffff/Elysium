using Elysium.Application.Features.Courses.DTOs;
using Microsoft.AspNetCore.Mvc;
using Elysium.Application.Features.Enrollments.Services;
using Elysium.Application.Features.Enrollments.DTOs;
using Microsoft.AspNetCore.Mvc.Infrastructure;
namespace Elysium.Api.Controllers;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController (IEnrollmentService enrollmentService): ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EnrollStudentRequest request, CancellationToken cancellationToken = default)
    {
        var result = await enrollmentService.EnrollStudentIntoCourse(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

}
