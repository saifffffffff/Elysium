using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Sessions.Services;

public interface ISessionService
{
    
    Task<Result<IReadOnlyCollection<SessionDto>>> GetAllByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(int sessionId, CancellationToken cancellationToken = default);
    
    Task<Result> EndAsync(int sessionId, CancellationToken cancellationToken = default);
    Task<Result<int>> StartAsync(StartSessionRequest request, CancellationToken cancellationToken = default);
    
    Task<Result<JoinSessionResponse>> JoinAsync(JoinSessionRequest request, CancellationToken cancellationToken = default);
    Task<Result> LeaveAsync(LeaveSessionRequest request, CancellationToken cancellationToken = default);
}
