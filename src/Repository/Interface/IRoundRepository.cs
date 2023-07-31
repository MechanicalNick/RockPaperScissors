using RockPaperScissors.Model.Dto;
using RockPaperScissors.Model.Entity;

namespace RockPaperScissors.Repository.Interface;

public interface IRoundRepository
{
    Task<RoundEntity> UpdateRoundAsync(int gameId, int round, Turn turn, bool isFirstPlayerTurn,
        CancellationToken ct);
    Task<RoundEntity> CreateRoundAsync(int gameId, int round, Turn turn, bool isFirstPlayer, CancellationToken ct);
    Task<IEnumerable<RoundEntity>> FindRoundsAsync(int gameId, CancellationToken ct);
}