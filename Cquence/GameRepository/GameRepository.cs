using System.Xml.Linq;
using valocity.assignment.cquence.domain;
using valocity.assignment.cquence.infrastructure.contacts.gamerepository;
using Microsoft.Extensions.Logging;

namespace valocity.assignment.cquence.Infrastructure.Persistence
{
    /// <summary>
    /// Implementation of IGameRepository that manages game state persistence using XML files.
    /// </summary>
    public class GameRepository : IGameRepository
    {
        private static readonly string _matchHistoryFolder = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Matches");
        private readonly ILogger<GameRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the GameRepository class.
        /// </summary>
        /// <param name="logger">The ILogger instance for logging.</param>
        public GameRepository(ILogger<GameRepository> logger)
        {
            _logger = logger;

            // Ensure the match history folder exists, if not, create it
            if (!Directory.Exists(_matchHistoryFolder))
            {
                Directory.CreateDirectory(_matchHistoryFolder);
                _logger.LogInformation("Match history folder created at {Path}", _matchHistoryFolder);
            }
        }

        /// <summary>
        /// Retrieves the game state asynchronously from an XML file.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>The retrieved GameState object or null if not found.</returns>
        public async Task<GameState> GetGameStateAsync(string gameId)
        {
            var gameHistoryFile = Path.Combine(_matchHistoryFolder, $"{gameId}.xml");

            if (!File.Exists(gameHistoryFile))
            {
                _logger.LogWarning("Game state file not found for GameId {GameId} at {Path}", gameId, gameHistoryFile);
                return null;
            }

            try
            {
                var doc = await Task.Run(() => XDocument.Load(gameHistoryFile));
                var gameState = new GameState
                {
                    GameId = doc.Root?.Element("GameId")?.Value,
                    UserId = doc.Root?.Element("UserId")?.Value,
                    CurrentLevel = int.TryParse(doc.Root?.Element("CurrentLevel")?.Value, out var level) ? level : 1,
                    CorrectSequence = doc.Root?.Element("CorrectSequence")?.Value?.Split(',').ToList() ?? new List<string>(),
                    SelectedCards = doc.Root?.Element("SelectedCards")?.Value?.Split(',').ToList() ?? new List<string>(),
                    SequenceHistory = doc.Root?.Elements("RoundHistory")
                        .Select(x => x.Element("CorrectSequence")?.Value?.Split(',').ToList())
                        .Where(seq => seq != null)
                        .ToList(),
                    IncorrectSequenceAttempt = doc.Root?.Element("IncorrectSequenceAttempt")?.Value
                };

                _logger.LogInformation("Game state successfully retrieved for GameId {GameId}", gameId);
                return gameState;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading game state for GameId {GameId}", gameId);
                return null;
            }
        }

        /// <summary>
        /// Saves the game state asynchronously to an XML file.
        /// </summary>
        /// <param name="gameState">The GameState object to be saved.</param>
        public async Task SaveGameStateAsync(GameState gameState)
        {
            var gameHistoryFile = Path.Combine(_matchHistoryFolder, $"{gameState.GameId}.xml");

            try
            {
                var doc = new XDocument(
                    new XElement("Game",
                        new XElement("GameId", gameState.GameId),
                        new XElement("UserId", gameState.UserId),
                        new XElement("CurrentLevel", gameState.CurrentLevel),
                        new XElement("CorrectSequence", string.Join(",", gameState.CorrectSequence)),
                        new XElement("SelectedCards", string.Join(",", gameState.SelectedCards)),
                        new XElement("IncorrectSequenceAttempt", gameState.IncorrectSequenceAttempt),
                        gameState.SequenceHistory.Select(history =>
                            new XElement("RoundHistory",
                                new XElement("CorrectSequence", string.Join(",", history))
                            )
                        )
                    )
                );

                await Task.Run(() => doc.Save(gameHistoryFile));
                _logger.LogInformation("Game state successfully saved for GameId {GameId} at {Path}", gameState.GameId, gameHistoryFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving game state for GameId {GameId}", gameState.GameId);
            }
        }
    }
}
