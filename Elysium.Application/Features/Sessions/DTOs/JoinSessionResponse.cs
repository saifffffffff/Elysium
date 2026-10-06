using System;
using System.Collections.Generic;
using System.Text;
using Elysium.Application.Features.Transcription.DTOs;

namespace Elysium.Application.Features.Sessions.DTOs;

public record JoinSessionResponse(int sessionStudentId , IEnumerable<TranscriptionSegmentDto> transcript );

