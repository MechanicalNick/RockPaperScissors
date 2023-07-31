using FluentMigrator.Runner;
using RockPaperScissors.Migration;

namespace RockPaperScissors.Infrastructure;

public class GameMigratorRunner : IGameMigratorRunner
{
    private readonly IConfiguration _configuration;
    
    public GameMigratorRunner(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Migrate()
    {
        var connectionString = _configuration.GetConnectionString("gameDb")!;
        var serviceProvider = CreateServices(connectionString);
        using var scope = serviceProvider.CreateScope();
        UpdateDatabase(scope.ServiceProvider);
    }
    
    private static IServiceProvider CreateServices(string connectionString)
    {
        var provider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithRunnerConventions(new MigrationRunnerConventions())
                .WithMigrationsIn(typeof(InitTablesMigration).Assembly)
            )
            .BuildServiceProvider(false);

        return provider;
    }
    
    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}