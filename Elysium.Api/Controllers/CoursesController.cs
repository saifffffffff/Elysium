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

    [HttpGet("{teacherId:int}")]
    public async Task<IActionResult> GetAllByTeacherId (int teacherId , CancellationToken cancellationToken) 
    {
        var result = await courseService.GetAllByTeacherId(teacherId);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<IActionResult> GetAllByStudentId(int studentId, CancellationToken cancellationToken)
    {
        var result = await courseService.GetAllByStudentId(studentId);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }


}
