using RockPaperScissors.Infrastructure;
using RockPaperScissors.Repository;
using RockPaperScissors.Repository.Impl;
using RockPaperScissors.Repository.Interface;
using RockPaperScissors.Service;

namespace RockPaperScissors;

public static class ConfigureServicesExtensions
{
    public static void Configure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSingleton<IConnectionFactory, PostgresConnectionFactory>();
        services.AddSingleton<IGameMigratorRunner, GameMigratorRunner>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IRoundRepository, RoundRepository>();
        services.AddScoped<IGameService, GameService>();
    }
}