using static RockPaperScissors.ConfigureServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure(builder.Configuration);

var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => "Rock Paper Scissors!");

app.Run();