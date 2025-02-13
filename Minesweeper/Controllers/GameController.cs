namespace Minesweeper.Controllers;

using Microsoft.AspNetCore.Mvc;
using Minesweeper.Models;
using Minesweeper.Services;
using System.Text.Json;

[Route("api")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly GameManagerService _gameService;

    public GameController(GameManagerService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("new")]
    public IActionResult CreateGame([FromBody] NewGameRequest request)
    {
        try
        {
            var gameInfo = _gameService.CreateGame(request);
            Console.WriteLine(JsonSerializer.Serialize(gameInfo, new JsonSerializerOptions { WriteIndented = true }));

            return Ok(gameInfo);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Error = ex.Message
            });
        } 
    }
    [HttpPost("turn")]
    public IActionResult MakeTurn([FromBody] GameTurnRequest request)
    {
        try
        {
            var gameInfo = _gameService.MakeTurn(request);
            Console.WriteLine(JsonSerializer.Serialize(gameInfo, new JsonSerializerOptions { WriteIndented = true }));
            return Ok(gameInfo);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new ErrorResponse 
            { 
                Error = ex.Message
            });
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Error = ex.Message
            });
        }
    }
}
