
using Elysium.Api.Hubs;
using Elysium.Application.Features.Courses.DTOs;
using Elysium.Application.Features.Courses.Services;
using Elysium.Application.Features.Enrollments.Services;
using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Application.Features.Sessions.Services;
using Elysium.Application.Features.Students.Services;
using Elysium.Application.Features.Teachers.Services;
using Elysium.Application.Features.Transcription.Interfaces;
using Elysium.Application.Features.Transcription.Services;
using Elysium.Application.Features.Users.DTOs;
using Elysium.Application.Features.Users.Services;
using Elysium.Application.Helpers;
using Elysium.Domain.Interfaces;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Elysium.Infrastructure.Options;
using Elysium.Infrastructure.Presistence;
using Elysium.Infrastructure.Presistence.Repositories;
using Elysium.Infrastructure.Presistence.UnitOfWork;
using Elysium.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{


    public static IServiceCollection AddDatabase(this IServiceCollection services , IConfiguration configuration)
    {


        services.AddDbContext<AppDbContext>(options =>
        {
            var constr = configuration.GetValue<string>("ConnectionString");
            
            options.UseSqlServer(constr);
        });

        
        return services;

    }
    public static IServiceCollection AddRepositories(this IServiceCollection services )
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IMaterialRepository, MaterialRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IStudentSessionRepository, StudentSessionRepository>();
        services.AddScoped<ITranscriptSegmentRepository, TranscriptSegmentRepository>();
        services.AddScoped<IConfusionFlagRepository, ConfusionFlagRepository>();
        services.AddScoped<IAiChatRepository, AiChatRepository>();
        services.AddScoped<IAiChatMessageRepository, AiChatMessageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }


    public static IServiceCollection AddApplicationServices(this IServiceCollection services )
    {

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<ICodeGenerator, CodeGenerator>();

        return services;
    }


    public static IServiceCollection AddValidators (this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        services.AddScoped<IValidator<SignInRequest>, SignInRequestValidator>();
        services.AddScoped<IValidator<UpdateProfileRequest>, UpdateProfileRequestValidator>();
        services.AddScoped<IValidator<ChangeUsernameRequest>, ChangeUsernameRequestValidator>();
        services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
        services.AddScoped<IValidator<CreateCourseRequest>, CreateCourseRequestValidator>();
        services.AddScoped<IValidator<CreateSessionRequest>, CreateSessionRequestValidator>();

        return services;
    }


    public static IServiceCollection AddRealtimeAndAudioServices(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddSingleton<ConnectionTracker>();
        services.AddSingleton<ITranscriptionProvider, DeepgramTranscriptionProvider>();
        services.AddScoped<ISessionNotifier, SessionNotifier>();
        services.AddScoped<ISpeechToTextService, SpeechToTextService>();

        services.Configure<SttOptions>(configuration.GetSection(SttOptions.SectionName));
        
        return services;

    }
}
