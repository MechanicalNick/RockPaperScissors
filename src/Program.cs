using Dapper;
using RockPaperScissors.Configuration;
using RockPaperScissors.Infrastructure;
using static RockPaperScissors.ConfigureServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure(builder.Configuration);

var app = builder.Build();

app.MapControllers();

DefaultTypeMap.MatchNamesWithUnderscores = true;

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = string.Empty;
    });
}

await app.RunAndMigrateAsync(app.Services.GetRequiredService<IGameMigratorRunner>());