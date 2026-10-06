using System.Text;
using Elysium.Application.Features.AiChat.DTOs;
using Elysium.Application.Features.Transcription.Interfaces;
using Elysium.Domain.Interfaces;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using FluentValidation;

namespace Elysium.Application.Features.AiChat.Services;


public class AiChatService(ILlmProvider llmService, ISessionRepository sessionRepository, IStudentSessionRepository studentSessionRepository, ITranscriptSegmentRepository transcriptSegmentRepository, IAiChatMessageRepository aiChatMessageRepository, IUnitOfWork unitOfWork, IValidator<StreamChatRequest> streamChatRequestValidator) : IAiChatService
{


    public async IAsyncEnumerable<Chunk> StreamChat(StreamChatRequest request)
    {
        // Rule 1 : request must be valid (studentSessionId > 0, question non-empty, <= 1000 chars)
        var validationResult = streamChatRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            yield return new Chunk { Content = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)), IsErrorMessage = true };
            yield break;
        }

        // Rule 2 : student session must exist
        var studentSession = await studentSessionRepository.GetByIdAsync(request.studentSessionId);

        if (studentSession is null)
        {
            yield return new Chunk { Content = $"student session with id {request.studentSessionId} not found.", IsErrorMessage = true }; 
            yield break;
        }

        // Rule 3 : student must currently be in session (not left)
        if (studentSession.HasLeft)
        {
            yield return new Chunk { Content = $"student has already left this session.", IsErrorMessage = true };
            yield break;
        }

        // Rule 4 : parent session must exist (linked via StudentSession.SessionId)
        var session = await sessionRepository.GetByIdAsync(studentSession.SessionId);

        if (session is null)
        {
            yield return new Chunk { Content = $"session with id {studentSession.SessionId} not found.", IsErrorMessage = true };
            yield break;
        }

        // Rule 5 : session must be live (not finished)
        if (session.IsFinished)
        {
            yield return new Chunk { Content = $"session with id {session.Id} already finished.", IsErrorMessage = true };
            yield break;
        }

        // Rule 6 : session must have transcript context for the LLM
        var transcriptSegments = await transcriptSegmentRepository.GetBySessionIdAsync(session.Id);

        if (transcriptSegments is null || transcriptSegments.Count == 0)
        {
            
            yield return new Chunk { Content = $"session with id {session.Id} has no transcript segments.", IsErrorMessage = true };
            yield break;
        }

        // Rule 7 : question must satisfy AiChatMessage domain invariants
        var messageResult = AiChatMessage.Create(request.studentSessionId, request.question);

        if (!messageResult.IsSuccess)
        {
            
            yield return new Chunk { Content = string.Join("; ", messageResult.Errors.Select(e => e.message)), IsErrorMessage = true };
            yield break;
        }

        var chatContext = await aiChatMessageRepository.GetAllBySessionStudentIdAsync(request.studentSessionId);
        
        var aiChatMessage = messageResult.Value!;

        await aiChatMessageRepository.AddAsync(aiChatMessage);
        await unitOfWork.SaveChangesAsync();

        //  stream grounded answer, then persist it
        var strResponse = new StringBuilder();
        

        await foreach (var chatStreamChunk in llmService.GenerateResponseAsync(request.question, transcriptSegments , chatContext))
        {
            if ( chatStreamChunk is not null )
            {
                strResponse.Append(chatStreamChunk);
                yield return new Chunk { Content = chatStreamChunk };
            }
        }

        aiChatMessage.SetAnswer(strResponse.ToString());
        
        await unitOfWork.SaveChangesAsync();
    }

}
