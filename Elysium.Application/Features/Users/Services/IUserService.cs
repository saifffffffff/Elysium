using Elysium.Application.Features.Users.DTOs;
using Elysium.Domain.Models;
using Elysium.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Users.Services;


public interface IUserService
{

    Task<Result<int>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default );
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<Result<SignInResponse>> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default);
    
    Task<Result> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result> ChangeUsernameAsync(ChangeUsernameRequest request, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
}



