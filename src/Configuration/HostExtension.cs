using FluentMigrator.Runner;
using RockPaperScissors.Infrastructure;

namespace RockPaperScissors.Configuration;

public static class HostExtension
{
    public static async Task RunAndMigrateAsync(this IHost host, IGameMigratorRunner runner)
    {
        runner.Migrate();
        await host.RunAsync();
    }
}