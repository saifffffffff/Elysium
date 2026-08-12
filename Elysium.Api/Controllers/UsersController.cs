using Elysium.Application.Features.Users.DTOs;
using Elysium.Application.Features.Users.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Elysium.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await userService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id  , CancellationToken cancellationToken)
    {
        var result = await userService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken = default )
    {
        var result = await userService.CreateAsync(request, cancellationToken);
        
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value }) : BadRequest(result.Errors);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete( int id , CancellationToken cancellationToken = default)
    {
        var result = await userService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound(result.Errors);
    }

    [HttpPost("signin")]
    public async Task <IActionResult> SignIn ( [FromBody] SignInRequest request , CancellationToken cancellationToken = default )
    {
        var result = await userService.SignInAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Errors);
    }


    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProfileRequest request , CancellationToken cancellationToken = default )
    {
        var result = await userService.UpdateProfileAsync(request, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }


}
