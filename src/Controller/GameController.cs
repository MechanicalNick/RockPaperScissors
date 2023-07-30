using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Service;

namespace RockPaperScissors.Controller;

[Route("api/v1/[controller]/[action]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Test()
    { 
        return Ok($"Test");
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromQuery] string userName, CancellationToken ct)
    { 
        return Ok($"Hello World, {userName}!");
    }
}