using Elysium.Application.Features.Courses.DTOs;
using Elysium.Application.Features.Courses.Services;
using Elysium.Application.Features.Students.Services;
using Elysium.Application.Features.Teachers.Services;
using Elysium.Application.Features.Users.DTOs;
using Elysium.Application.Features.Users.Services;
using Elysium.Application.Helpers;
using Elysium.Domain.Interfaces;
using Elysium.Domain.Interfaces.Repositories;
using Elysium.Domain.Models;
using Elysium.Infrastructure.Presistence;
using Elysium.Infrastructure.Presistence.Repositories;
using Elysium.Infrastructure.Presistence.UnitOfWork;
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

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
builder.Services.AddScoped<IValidator<SignInRequest>, SignInRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateProfileRequest>, UpdateProfileRequestValidator>();
builder.Services.AddScoped<IValidator<ChangeUsernameRequest>, ChangeUsernameRequestValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
builder.Services.AddScoped<IValidator<CreateCourseRequest>, CreateCourseValidator>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddScoped<ICodeGenerator, CodeGenerator>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => "Elysium API is running");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();