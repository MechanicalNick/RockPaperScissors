using RockPaperScissors.Model.Dto;

namespace RockPaperScissors.Service;

public interface IGameService
{
    Task<Tuple<CreateResult, CreateResponse>> CreateGameAsync(string playerName, CancellationToken ct);
    Task<Tuple<JoinResult, JoinResponse>> JoinGameAsync(int gameId, string playerName, CancellationToken ct);
    Task<Tuple<TurnResult, TurnResponse>> TurnAsync(int gameId, int userId, Turn turn, CancellationToken ct);
    Task<StatisticResponse> GetStatisticAsync(int gameId, CancellationToken ct);
    Task<Tuple<JoinResult, JoinResponse>> StartWithBotAsync(int gameId, CancellationToken ct);
}