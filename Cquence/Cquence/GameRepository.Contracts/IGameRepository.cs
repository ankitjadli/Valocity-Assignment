using valocity.assignment.cquence.domain;

namespace valocity.assignment.cquence.infrastructure.contacts.gamerepository
{
    /// <summary>
    /// Defines the contract for repository operations related to the game state.
    /// </summary>
    public interface IGameRepository
    {
        /// <summary>
        /// Retrieves the state of a game by its unique identifier.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>The game state associated with the specified game ID.</returns>
        Task<GameState> GetGameStateAsync(string gameId);

        /// <summary>
        /// Saves the current state of a game.
        /// </summary>
        /// <param name="gameState">The game state to save.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task SaveGameStateAsync(GameState gameState);
    }
}
