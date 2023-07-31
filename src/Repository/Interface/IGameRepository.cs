using RockPaperScissors.Model.Entity;

namespace RockPaperScissors.Repository.Interface;

public interface IGameRepository
{
    Task<GameEntity> CreateGameAsync(int playerId, CancellationToken ct);
    Task<GameEntity?> FindGameAsync(int gameId, CancellationToken ct);
    Task JoinGameAsync(int playerId, int gameId, CancellationToken ct);
    Task StartWithBotAsync(int botId, int gameId, CancellationToken ct);
}