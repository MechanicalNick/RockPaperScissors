using Dapper;
using RockPaperScissors.Infrastructure;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Repository.Interface;

namespace RockPaperScissors.Repository.Impl;

public class GameRepository : IGameRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public GameRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GameEntity> CreateGameAsync(int playerId, CancellationToken ct)
    {
        await using var connection = _connectionFactory.GetConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("PlayerId", playerId);
        
        var saveQuery = @"
        insert into public.game (first_player_id, rounds_count)
        values (@PlayerId, 0) returning id;";
    
        var gameEntity = await connection.QuerySingleAsync<GameEntity>(new CommandDefinition(saveQuery,
            parameters:parameters, cancellationToken: ct));
        return gameEntity;
    }
    
    public async Task<GameEntity?> FindGameAsync(int gameId, CancellationToken ct)
    {
        await using var connection = _connectionFactory.GetConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("GameId", gameId);
        
        var findQuery = @"select * from game where id=@GameId limit 1";
    
        var gameEntity = await connection.QuerySingleOrDefaultAsync<GameEntity>(new CommandDefinition(findQuery,
            parameters:parameters, cancellationToken: ct));
        return gameEntity;
    }
    
    public async Task JoinGameAsync(int playerId, int gameId, CancellationToken ct)
    {
        await using var connection = _connectionFactory.GetConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("PlayerId", playerId);
        parameters.Add("GameId", gameId);
        
        var saveQuery = @"
        update public.game
	    set second_player_id=@PlayerId
	    where id=@GameId;";

        await connection.ExecuteAsync(new CommandDefinition(saveQuery,
            parameters:parameters, cancellationToken: ct));
    }

    public async Task StartWithBotAsync(int botId, int gameId, CancellationToken ct)
    {
        await using var connection = _connectionFactory.GetConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("PlayerId", botId);
        parameters.Add("GameId", gameId);
        
        var saveQuery = @"
        update public.game
	    set with_bot=true
	    where id=@GameId;";

        await connection.ExecuteAsync(new CommandDefinition(saveQuery,
            parameters:parameters, cancellationToken: ct));
    }
}