using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Sessions.Services;

public interface ISessionService
{
    Task<Result<int>> CreateAsync(CreateSessionRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<SessionDto>>> GetAllByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Result> EndAsync(int sessionId, CancellationToken cancellationToken = default);
}
