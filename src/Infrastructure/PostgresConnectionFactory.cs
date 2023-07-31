using System.Data;
using System.Data.Common;
using Npgsql;

namespace RockPaperScissors.Infrastructure;

public class PostgresConnectionFactory : IConnectionFactory
{
    private readonly IConfiguration _configuration;

    public PostgresConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public DbConnection GetConnection()
    {
        var connectionString = _configuration.GetConnectionString("gameDb")!;
        return new NpgsqlConnection(connectionString);
    }
}