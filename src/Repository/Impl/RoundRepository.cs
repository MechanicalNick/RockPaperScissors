using Dapper;
using RockPaperScissors.Infrastructure;
using RockPaperScissors.Model.Dto;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Repository.Interface;

namespace RockPaperScissors.Repository.Impl;

public class RoundRepository : IRoundRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public RoundRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<RoundEntity>> FindRoundsAsync(int gameId, CancellationToken ct)
    {
        await using var connection = _connectionFactory.GetConnection();

        var parameters = new DynamicParameters();
        parameters.Add("GameId", gameId);

        var findQuery = @"select * from round where game_id=@GameId";

        var gameEntities = await connection.QueryAsync<RoundEntity>(new CommandDefinition(findQuery,
            parameters: parameters, cancellationToken: ct));
        return gameEntities;
    }

    public async Task<RoundEntity> UpdateRoundAsync(int gameId, int round, Turn turn, bool isFirstPlayer,
        CancellationToken ct)
    {
        await using var connection = _connectionFactory.GetConnection();

        var parameters = new DynamicParameters();
        parameters.Add("Round", round);
        parameters.Add("GameId", gameId);
        parameters.Add("Turn", (int) turn);

        var playerValue = isFirstPlayer ? "first_player_choice" : "second_player_choice";

        var saveQuery = $@"
        update public.round
	    set {playerValue}=@Turn 
        where game_id=@GameId and number=@Round returning *;";

        var entity = await connection.QuerySingleAsync<RoundEntity>(new CommandDefinition(saveQuery,
            parameters: parameters, cancellationToken: ct));

        return entity;
    }

    public async Task<RoundEntity> CreateRoundAsync(int gameId, int round, Turn turn,
        bool isFirstPlayer, CancellationToken ct)
    {
        await using var connection = _connectionFactory.GetConnection();

        var parameters = new DynamicParameters();
        parameters.Add("Round", round);
        parameters.Add("GameId", gameId);
        parameters.Add("Turn", (int) turn);

        var playerValue = isFirstPlayer ? "first_player_choice" : "second_player_choice";

        var saveQuery = $@"
        insert into public.round (game_id, number, {playerValue})
        values (@GameId, @Round, @Turn) returning *";

        var entity = await connection.QuerySingleAsync<RoundEntity>(new CommandDefinition(saveQuery,
            parameters: parameters, cancellationToken: ct));

        return entity;
    }
}