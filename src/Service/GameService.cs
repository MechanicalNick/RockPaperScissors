using RockPaperScissors.Infrastructure;
using RockPaperScissors.Model.Dto;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Repository.Interface;

namespace RockPaperScissors.Service;

public class GameService : IGameService
{
    private readonly Random _random = new();
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IRoundRepository _roundRepository;
    private readonly IConfiguration _configuration;

    public GameService(IGameRepository gameRepository, IPlayerRepository playerRepository,
        IRoundRepository roundRepository, IConfiguration configuration)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _roundRepository = roundRepository;
        _configuration = configuration;
    }

    public async Task<Tuple<CreateResult, CreateResponse>> CreateGameAsync(string playerName, CancellationToken ct)
    {
        if(string.Equals("pc", playerName, StringComparison.InvariantCultureIgnoreCase))
            return Tuple.Create(CreateResult.IsBot, new CreateResponse(0,0));
        
        var playerId = await FindOrCreatePlayerAsync(playerName, ct);
        var game = await _gameRepository.CreateGameAsync(playerId, ct);
        return Tuple.Create(CreateResult.Ok, new CreateResponse(game.Id, playerId));
    }

    public async Task<Tuple<JoinResult, JoinResponse>> JoinGameAsync(int gameId, string playerName,
        CancellationToken ct)
    {
        var playerId = await FindOrCreatePlayerAsync(playerName, ct);
        return await JoinGameAsync(gameId, playerName, playerId, ct);
    }

    public async Task<Tuple<TurnResult, TurnResponse>> TurnAsync(int gameId, int playerId, 
        Turn turn, CancellationToken ct)
    {
        var game = await _gameRepository.FindGameAsync(gameId, ct);
        if (game == null)
            return Tuple.Create(TurnResult.GameNotFound, new TurnResponse());

        var player = await _playerRepository.FindPlayerByIdAsync(playerId, ct);
        if (player == null || player.IsBot)
            return Tuple.Create(TurnResult.WrongId, new TurnResponse());
        
        var playersTurn = await TurnAsyncImpl(game, playerId, turn, ct);
        if (playersTurn.Item1 == TurnResult.WaitOtherPlayer && game.WithBot)
        {
            var bot = await _playerRepository.GetBotAsync(ct);
            var length = Enum.GetNames(typeof(Turn)).Length;
            var botTurn = (Turn) _random.Next(0, length);
            return await TurnAsyncImpl(game, bot.Id, botTurn, ct);
        }

        return playersTurn;
    }

    public async Task<StatisticResponse> GetStatisticAsync(int gameId, CancellationToken ct)
    {
        var entities = await _roundRepository.FindRoundsAsync(gameId, ct);
        var rounds = entities.ToList();
        var items = rounds
            .OrderBy(r => r.Number)
            .Select(r => new StatisticItem(
                Number: r.Number,
                FirstChoice: EnumHelper.ToReadableValue<Turn>(r.FirstPlayerChoice),
                SecondChoice: EnumHelper.ToReadableValue<Turn>(r.SecondPlayerChoice),
                TurnResponse: EnumHelper.ToReadableValue<TurnResponse>(
                    TurnSolver.SolveNullable(r.FirstPlayerChoice, r.SecondPlayerChoice)
                )
            ))
            .ToList();
        
        var lookup = rounds
            .Where(r => r.FirstPlayerChoice.HasValue && r.SecondPlayerChoice.HasValue)
            .ToLookup(r => TurnSolver
                .Solve((Turn) r.FirstPlayerChoice.Value, (Turn) r.SecondPlayerChoice.Value));
        
        return new StatisticResponse(
            Result: items,
            FirstWinsCount: lookup[TurnResponse.FirstPlayerWins].Count(),
            SecondWinsCount: lookup[TurnResponse.SecondPlayerWins].Count(),
            DrawCount: lookup[TurnResponse.Draw].Count()
        );
    }

    public async Task<Tuple<JoinResult, JoinResponse>> StartWithBotAsync(int gameId, CancellationToken ct)
    {
        var botId = await FindOrCreatePlayerAsync("pc", ct);
        var game = await _gameRepository.FindGameAsync(gameId, ct);
        
        if(game == null)
            return Tuple.Create(JoinResult.GameNotFound, new JoinResponse());
        if (game.SecondPlayerId.HasValue)
            return Tuple.Create(JoinResult.GameIsFull, new JoinResponse());
        
        await _gameRepository.StartWithBotAsync(botId, gameId, ct);
        return await JoinGameAsync(gameId, "pc", botId, ct);
    }
    
    private async Task<Tuple<TurnResult, TurnResponse>> TurnAsyncImpl(GameEntity game, int playerId, 
        Turn turn, CancellationToken ct)
    {
        if (!(playerId == game.FirstPlayerId || playerId == game.SecondPlayerId))
            return Tuple.Create(TurnResult.WrongId, new TurnResponse());

        var currentRound = await FindCurrentRoundAsync(game.Id, ct);
        var maxNumber = currentRound?.Number ?? 0;
        var isFirstPlayerTurn = playerId == game.FirstPlayerId;

        var needCreateNewRound = currentRound == null ||
                                 (currentRound.FirstPlayerChoice != null &&
                                  currentRound.SecondPlayerChoice != null);
        if (needCreateNewRound)
            return await CreateNewRoundAsync(game, turn, maxNumber, isFirstPlayerTurn, ct);

        var needUpdate = (isFirstPlayerTurn && currentRound.FirstPlayerChoice == null) ||
                         (!isFirstPlayerTurn && currentRound.SecondPlayerChoice == null);
        if (needUpdate)
            return await UpdateCurrentRoundAsync(game.Id, currentRound, turn, isFirstPlayerTurn, ct);

        return Tuple.Create(TurnResult.WaitOtherPlayer, new TurnResponse());
    }

    private async Task<Tuple<TurnResult, TurnResponse>> CreateNewRoundAsync(GameEntity game, Turn turn, 
         int maxNumber, bool isFirstPlayerTurn, CancellationToken ct)
    {
        var maxRounds = _configuration.GetValue<int>("GameSettings:MaxRounds");
        if (game.RoundsCount >= maxRounds)
            return Tuple.Create(TurnResult.EndGame, new TurnResponse());
        return await CreateNewRoundAsync(game.Id, maxNumber + 1, turn, isFirstPlayerTurn, ct);
    }

    private async Task<Tuple<JoinResult, JoinResponse>> JoinGameAsync(int gameId, string playerName,
        int playerId, CancellationToken ct)
    {
        var game = await _gameRepository.FindGameAsync(gameId, ct);
        if (game == null)
            return Tuple.Create(JoinResult.GameNotFound, new JoinResponse());
        
        var canJoin = game.SecondPlayerId == null || playerName.Equals("pc");
        if (!canJoin)
            return Tuple.Create(JoinResult.GameIsFull, new JoinResponse());
        
        if (game.FirstPlayerId == playerId)
            return Tuple.Create(JoinResult.PlayerExists, new JoinResponse());

        await _gameRepository.JoinGameAsync(playerId, gameId, ct);
        return Tuple.Create(JoinResult.Ok, new JoinResponse(playerId));
    }
    
    private async Task<RoundEntity?> FindCurrentRoundAsync(int gameId, CancellationToken ct)
    {
        var findRounds = await _roundRepository.FindRoundsAsync(gameId, ct);
        var rounds = findRounds.ToList();
        RoundEntity? currentRound = null;
        if (rounds.Any())
        {
            var maxNumber = rounds.Max(r => r.Number);
            currentRound = rounds.FirstOrDefault(r => r.Number == maxNumber);
        }
        return currentRound;
    }
    
    private async Task<Tuple<TurnResult, TurnResponse>> UpdateCurrentRoundAsync(int gameId, RoundEntity round,
        Turn turn, bool isFirstPlayerTurn, CancellationToken ct)
    {
        if ((isFirstPlayerTurn && round.FirstPlayerChoice != null) ||
            (!isFirstPlayerTurn && round.SecondPlayerChoice != null))
            return Tuple.Create(TurnResult.WaitOtherPlayer, new TurnResponse());

        round = await _roundRepository.UpdateRoundAsync(gameId, round.Number, turn, isFirstPlayerTurn, ct);

        return Tuple.Create(TurnResult.Ok, TurnSolver.Solve(
            (Turn) round.FirstPlayerChoice.Value,
            (Turn) round.SecondPlayerChoice.Value));
    }

    private async Task<Tuple<TurnResult, TurnResponse>> CreateNewRoundAsync(int gameId, int roundNumber, Turn turn,
        bool isFirstPlayer, CancellationToken ct)
    {
        await _roundRepository.CreateRoundAsync(gameId, roundNumber, turn, isFirstPlayer, ct);
        return Tuple.Create(TurnResult.WaitOtherPlayer, new TurnResponse());
    }

    private async Task<int> FindOrCreatePlayerAsync(string playerName, CancellationToken ct)
    {
        var player = await _playerRepository.FindPlayerByNameAsync(playerName, ct) ??
                     await _playerRepository.SavePlayerAsync(playerName, ct);
        return player.Id;
    }
}