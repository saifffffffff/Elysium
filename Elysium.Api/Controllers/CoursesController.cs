using Elysium.Application.Features.Courses.DTOs;
using Elysium.Application.Features.Courses.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elysium.Api.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController(ICourseService courseService) : ControllerBase
{


    [HttpPost]
    public async Task<IActionResult> Create( CreateCourseRequest request , CancellationToken cancellationToken)
    {
        var result = await courseService.CreateAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }



}
