using valocity.assignment.cquence.domain;
namespace valocity.assignment.cquence.core.gameengine.IServices
{
    /// <summary>
    /// This interface provides methods to manage the game flow, including starting the game, submitting sequences, and retrieving game states.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Starts a new game for the given user.
        /// </summary>
        /// <param name="userId">The unique identifier for the user starting the game.</param>
        /// <returns>The initial game state with game details, such as game ID, level, and correct sequence.</returns>
        Task<GameState> StartGameAsync(string userId);

        /// <summary>
        /// Submits a sequence of selected cards by the user for a specific game.
        /// </summary>
        /// <param name="gameId">The unique identifier for the game being played.</param>
        /// <param name="userSequence">The sequence of cards selected by the user.</param>
        /// <returns>The updated game state after submitting the sequence.</returns>
        Task<GameState> SubmitSequenceAsync(string gameId, List<string> userSequence);

        /// <summary>
        /// Retrieves the current state of the game.
        /// </summary>
        /// <param name="gameId">The unique identifier for the game whose state is to be retrieved.</param>
        /// <returns>The current game state, including level, selected cards, and sequence history.</returns>
        Task<GameState> GetGameStateAsync(string gameId);
    }
}
