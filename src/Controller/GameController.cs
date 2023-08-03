using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Model.Dto;
using RockPaperScissors.Service;

namespace RockPaperScissors.Controller;

[Route("api/v1")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Cоздает новую игру по имени игрока, возвращает код игрока и игры
    /// </summary>
    [HttpPost]
    [Route("game/create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromQuery] string playerName, CancellationToken ct)
    {
        var result = await _gameService.CreateGameAsync(playerName, ct);
        return result.Item1 switch
        {
            CreateResult.Ok => Ok(result.Item2),
            CreateResult.IsBot => Conflict($"Name {playerName} is reserved"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Подключает второго игрока по коду игры и имени игрока и возвращает код игрока
    /// </summary>
    [HttpPatch]
    [Route("game/{gameId}/join/{playerName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Join([FromRoute] int gameId, [FromRoute] string playerName,
        CancellationToken ct)
    {
        var result = await _gameService.JoinGameAsync(gameId, playerName, ct);
        return result.Item1 switch
        {
            JoinResult.Ok => Ok(result.Item2),
            JoinResult.PlayerExists => Conflict($"{playerName} already exists in game {gameId}"),
            JoinResult.GameIsFull => Conflict($"{gameId} is full"),
            JoinResult.GameNotFound => NotFound($"{gameId} not found"),
            _ => throw new ArgumentOutOfRangeException(nameof(Join))
        };
    }

    /// <summary>
    /// Ход игрока
    /// </summary>
    [HttpPost]
    [Route("game/{gameId}/player/{playerId}/{turn}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Turn([FromRoute] int gameId, [FromRoute] int playerId, [FromRoute] Turn turn,
        CancellationToken ct)
    {
        var result = await _gameService.TurnAsync(gameId, playerId, turn, ct);
        return result.Item1 switch
        {
            TurnResult.Ok => Ok(result.Item2.ToString()),
            TurnResult.WaitOtherPlayer => Ok("Wait second turn"),
            TurnResult.EndGame => Ok("Game over"),
            TurnResult.WrongId => UnprocessableEntity($"Wrong id {playerId}"),
            TurnResult.GameNotFound => NotFound($"Game {gameId} not found"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Получить статистику
    /// </summary>
    [HttpGet]
    [Route("game/{gameId}/statistic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Statistic([FromRoute] int gameId, CancellationToken ct)
    {
        var result = await _gameService.GetStatisticAsync(gameId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Начать с ботом
    /// </summary>
    [HttpPatch]
    [Route("game/{gameId}/start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StartWithBot([FromRoute] int gameId, CancellationToken ct)
    {
        var result = await _gameService.StartWithBotAsync(gameId, ct);
        return Ok(result.Item1.ToString());
    }
}