using Elysium.Api.Hubs;
using Elysium.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
 

builder.Services.AddOpenApi();

builder.Services.AddSignalR();

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddRepositories();
builder.Services.AddValidators();
builder.Services.AddRealtimeServices(builder.Configuration);

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

app.MapGet("test/stt-configuration", ( IOptions<SttOptions> options ) =>
{
    var config = options.Value;
    var apiKey = builder.Configuration["SpeechToText:Deepgram:ApiKey"];
    Console.WriteLine($"Deepgram ApiKey present: {apiKey}");
    Console.WriteLine("service : " + config.ActiveProvider);

    return new
    {
        ActiveProvider = config.ActiveProvider,
        DeepgramOptions = config.Deepgram,
        Defaults = config.Defaults            
    };
});

app.MapControllers();

app.Run();


