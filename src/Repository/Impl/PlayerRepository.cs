using Dapper;
using RockPaperScissors.Infrastructure;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Repository.Interface;

namespace RockPaperScissors.Repository.Impl;

public class PlayerRepository : IPlayerRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public PlayerRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<PlayerEntity?> FindPlayerByNameAsync(string playerName, CancellationToken ct)
    {
        await using var connection = _connectionFactory.GetConnection();
        
        var containsQuery = @"select * from player where name=@PlayerName limit 1;";
        var parameters = new DynamicParameters();
        parameters.Add("PlayerName", playerName);
        
        var playerEntity = await connection.QuerySingleOrDefaultAsync<PlayerEntity?>(new CommandDefinition(containsQuery,
            parameters:parameters, cancellationToken: ct));
        return playerEntity;
    }

    public async Task<PlayerEntity> GetBotAsync(CancellationToken ct)
    {
        await using var connection = _connectionFactory.GetConnection();
        
        var containsQuery = @"select * from player where is_bot=true limit 1;";
        
        return await connection.QuerySingleAsync<PlayerEntity>(new CommandDefinition(containsQuery, cancellationToken: ct));
    }


    public async Task<PlayerEntity> SavePlayerAsync(string playerName, CancellationToken ct)
    {
        await using var connection = _connectionFactory.GetConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("PlayerName", playerName);
        
        var saveQuery = @"
        insert into public.player (name, is_bot)
        values (@playerName, false) returning id;";
    
        var playerEntity = await connection.QuerySingleAsync<PlayerEntity>(new CommandDefinition(saveQuery,
            parameters:parameters, cancellationToken: ct));
        return playerEntity;
    }
}