using Elysium.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Sessions.DTOs;

public record class SessionDto(int id , string name , string? description , SessionStatus status , DateTime startedAt , DateTime? finishedAt );

