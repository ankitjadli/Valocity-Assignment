using Microsoft.Extensions.Logging;
using valocity.assignment.cquence.core.gameengine.IServices;
using valocity.assignment.cquence.infrastructure.contacts.gamerepository;
using valocity.assignment.cquence.domain;

namespace valocity.assignment.cquence.Application.Services
{
    /// <summary>
    /// Provides the implementation of the game service, including functionality to start a new game, 
    /// retrieve game state, and handle user sequence submissions.
    /// </summary>
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GameService> _logger;

        /// <summary>
        /// Initializes a new instance of the GameService class.
        /// </summary>
        /// <param name="gameRepository">The IGameRepository instance for game data management.</param>
        /// <param name="logger">The ILogger instance for logging.</param>
        public GameService(IGameRepository gameRepository, ILogger<GameService> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        /// <summary>
        /// Starts a new game for the given user by generating an initial game state.
        /// </summary>
        /// <param name="userId">The unique identifier of the user starting the game.</param>
        /// <returns>The initial game state.</returns>
        public async Task<GameState> StartGameAsync(string userId)
        {
            _logger.LogInformation("Starting a new game for user {UserId}", userId);

            var gameId = Guid.NewGuid().ToString();
            var gameState = new GameState
            {
                GameId = gameId,
                UserId = userId,
                CurrentLevel = 1,
                CorrectSequence = GenerateSequence(1),
                SelectedCards = new List<string>(),
                SequenceHistory = new List<List<string>>(),
                IncorrectSequenceAttempt = null
            };

            // Save the initial game state to the repository
            await _gameRepository.SaveGameStateAsync(gameState);
            _logger.LogInformation("Game started with GameId {GameId} for user {UserId}", gameId, userId);

            return gameState;
        }

        /// <summary>
        /// Retrieves the current game state by game ID.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>The current game state.</returns>
        /// <exception cref="Exception">Thrown if the game is not found.</exception>
        public async Task<GameState> GetGameStateAsync(string gameId)
        {
            _logger.LogInformation("Retrieving game state for GameId {GameId}", gameId);

            var gameState = await _gameRepository.GetGameStateAsync(gameId);
            if (gameState == null)
            {
                _logger.LogWarning("Game with GameId {GameId} not found", gameId);
                throw new Exception("Game not found");
            }

            _logger.LogInformation("Game state retrieved for GameId {GameId}", gameId);
            return gameState;
        }

        /// <summary>
        /// Submits the user's sequence for validation and updates the game state accordingly.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <param name="userSequence">The sequence submitted by the user.</param>
        /// <returns>The updated game state.</returns>
        /// <exception cref="Exception">Thrown for incorrect sequence length or invalid sequences.</exception>
        public async Task<GameState> SubmitSequenceAsync(string gameId, List<string> userSequence)
        {
            _logger.LogInformation("Submitting sequence for GameId {GameId}", gameId);

            var gameState = await _gameRepository.GetGameStateAsync(gameId);
            if (gameState == null)
            {
                _logger.LogWarning("Game with GameId {GameId} not found during sequence submission", gameId);
                throw new Exception("Game not found");
            }

            // Initialize sequence history if null
            if (gameState.SequenceHistory == null)
                gameState.SequenceHistory = new List<List<string>>();

            // Add the correct sequence of the current level to history
            gameState.SequenceHistory.Add(gameState.CorrectSequence);

            // Validate sequence length
            if (userSequence.Count != gameState.CurrentLevel)
            {
                _logger.LogWarning("Incorrect number of cards selected for GameId {GameId}", gameId);
                throw new Exception("Incorrect number of cards selected");
            }

            // Validate if the user's sequence matches the correct sequence
            if (userSequence.SequenceEqual(gameState.CorrectSequence))
            {
                _logger.LogInformation("Correct sequence submitted for GameId {GameId}", gameId);

                // Correct sequence, advance to the next level
                gameState.CurrentLevel++;
                gameState.CorrectSequence = GenerateSequence(gameState.CurrentLevel);
                await _gameRepository.SaveGameStateAsync(gameState);
            }
            else
            {
                // Incorrect sequence, reset selected cards and log the incorrect attempt
                gameState.SelectedCards.Clear();
                gameState.IncorrectSequenceAttempt = string.Join(",", userSequence);
                await _gameRepository.SaveGameStateAsync(gameState);

                _logger.LogWarning(
                    "Incorrect sequence submitted for GameId {GameId}. UserSequence: {UserSequence}, Expected: {CorrectSequence}",
                    gameId, string.Join(",", userSequence), string.Join(",", gameState.CorrectSequence));

                throw new Exception($"{gameState.IncorrectSequenceAttempt};{string.Join(",", gameState.CorrectSequence)}");
            }

            return gameState;
        }

        /// <summary>
        /// Generates a random sequence of cards based on the current game level.
        /// </summary>
        /// <param name="level">The current game level, determining the sequence length.</param>
        /// <returns>A list of card sequences.</returns>
        private List<string> GenerateSequence(int level)
        {
            var suits = new[] { "clubs", "diamonds", "hearts", "spades" };
            var ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            var random = new Random();

            var sequence = new List<string>();
            for (int i = 0; i < level; i++)
            {
                var suit = suits[random.Next(suits.Length)];
                var rank = ranks[random.Next(ranks.Length)];
                sequence.Add($"{suit}_{rank}");
            }

            _logger.LogDebug("Generated sequence for level {Level}: {Sequence}", level, string.Join(", ", sequence));
            return sequence;
        }
    }
}
