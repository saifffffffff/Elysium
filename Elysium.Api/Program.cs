using Elysium.Api.Hubs;
using Elysium.Application.Features.Courses.DTOs;
using Elysium.Application.Features.Courses.Services;
using Elysium.Application.Features.Enrollments.Services;
using Elysium.Application.Features.Sessions.DTOs;
using Elysium.Application.Features.Sessions.Services;
using Elysium.Application.Features.Students.Services;
using Elysium.Application.Features.Teachers.Services;
using Elysium.Application.Features.Transcription.Interfaces;
using Elysium.Application.Features.Transcription.Options;
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
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    var constr = config.GetSection("constr").Value;
    
    options.UseSqlServer(constr);

});

builder.Services.AddOpenApi();

builder.Services.AddSignalR();
builder.Services.AddScoped<ISessionNotifier, SessionNotifier>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IStudentSessionRepository, StudentSessionRepository>();
builder.Services.AddScoped<ITranscriptSegmentRepository, TranscriptSegmentRepository>();
builder.Services.AddScoped<IConfusionFlagRepository, ConfusionFlagRepository>();
builder.Services.AddScoped<IAiChatRepository, AiChatRepository>();
builder.Services.AddScoped<IAiChatMessageRepository, AiChatMessageRepository>();
builder.Services.AddScoped<ISpeechToTextService, SpeechToTextService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.Configure<TranscriptionStreamOptions>(options => {
    options.Model = "nova-2";
    options.Language = "en-US";
    options.SampleRate = 16000;
    options.Provider = "deepgram";
    options.EndpointingMs = 5000;
});

; // skip for now 
builder.Services.Configure<DeepgramOptions>(builder.Configuration.GetSection("Deepgram"));

builder.Services.AddSingleton<ConnectionTracker>();
builder.Services.AddSingleton<ITranscriptionProvider, DeepgramTranscriptionProvider>();


builder.Services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
builder.Services.AddScoped<IValidator<SignInRequest>, SignInRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateProfileRequest>, UpdateProfileRequestValidator>();
builder.Services.AddScoped<IValidator<ChangeUsernameRequest>, ChangeUsernameRequestValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
builder.Services.AddScoped<IValidator<CreateCourseRequest>, CreateCourseRequestValidator>();
builder.Services.AddScoped<IValidator<CreateSessionRequest>, CreateSessionRequestValidator>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ICodeGenerator, CodeGenerator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => "Elysium API is running");

app.MapHub<CourseHub>("/hub/course");
app.MapHub<SessionHub>("/hub/session");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();