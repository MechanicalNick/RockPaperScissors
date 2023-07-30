using RockPaperScissors.Infrastructure;
using RockPaperScissors.Service;

namespace RockPaperScissors;

public static class ConfigureServicesExtensions
{
    public static void Configure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddScoped<IGameService, GameService>();
        services.AddSingleton<IGameMigratorRunner, GameMigratorRunner>();
    }
}