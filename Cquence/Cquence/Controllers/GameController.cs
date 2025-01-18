using Microsoft.AspNetCore.Mvc;
using valocity.assignment.cquence.core.gameengine.IServices;
using valocity.assignment.cquence.domain;

namespace valocity.assignment.cquence.Controllers
{
    /// <summary>
    /// Provides endpoints for managing game operations such as starting a game, retrieving game state
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        /// <summary>
        /// Initializes a new instance of the GameController class.
        /// </summary>
        /// <param name="gameService">The IGameService instance for game operations.</param>
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Starts a new game for the given user.
        /// </summary>
        /// <param name="userId">The ID of the user who is starting the game.</param>
        /// <returns>The initial state of the game.</returns>
        [HttpPost("start")]
        public async Task<ActionResult<GameState>> StartGame([FromBody] string userId)
        {
            var gameState = await _gameService.StartGameAsync(userId);
            return Ok(gameState);
        }

        /// <summary>
        /// Retrieves the current state of a game by its ID.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>The current state of the game, or a 404 response if not found.</returns>
        [HttpGet("{gameId}/state")]
        public async Task<ActionResult<GameState>> GetGameState(string gameId)
        {
            var gameState = await _gameService.GetGameStateAsync(gameId);
            if (gameState == null)
                return NotFound("Game not found");
            return Ok(gameState);
        }

        /// <summary>
        /// Submits a sequence of user inputs for a specific game and evaluates the results.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <param name="userSequence">The sequence submitted by the user.</param>
        /// <returns>The updated state of the game after evaluating the sequence.</returns>
        /// <remarks>
        /// Handles exceptions that may occur during sequence submission, such as invalid inputs or 
        /// game state errors, and returns a BadRequest response with the error message.
        /// </remarks>
        [HttpPost("{gameId}/submit")]
        public async Task<ActionResult<GameState>> SubmitSequence(string gameId, [FromBody] List<string> userSequence)
        {
            try
            {
                var gameState = await _gameService.SubmitSequenceAsync(gameId, userSequence);
                return Ok(gameState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
