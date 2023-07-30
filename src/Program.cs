using RockPaperScissors.Configuration;
using RockPaperScissors.Infrastructure;
using static RockPaperScissors.ConfigureServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure(builder.Configuration);

var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => "Rock Paper Scissors!");

await app.RunAndMigrateAsync(app.Services.GetRequiredService<IGameMigratorRunner>());