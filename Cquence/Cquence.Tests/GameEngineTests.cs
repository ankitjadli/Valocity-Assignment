using Microsoft.Extensions.Logging;
using Moq;
using valocity.assignment.cquence.Application.Services;
using valocity.assignment.cquence.core.gameengine.IServices;
using valocity.assignment.cquence.domain;
using valocity.assignment.cquence.infrastructure.contacts.gamerepository;

namespace Cquence.Tests
{
    /// <summary>
    /// Unit tests for the Game Engine Service.
    /// </summary>
    public class GameEngineTests
    {
        private readonly IGameService _gameService;
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<ILogger<GameService>> _mockGameServiceLogger;

        /// <summary>
        /// Initializes test dependencies.
        /// </summary>
        public GameEngineTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _mockGameServiceLogger = new Mock<ILogger<GameService>>();
            _gameService = new GameService(_gameRepositoryMock.Object, _mockGameServiceLogger.Object);
        }

        /// <summary>
        /// Tests that starting a game with a valid user ID returns a valid game state.
        /// </summary>
        [Fact]
        public async Task StartGameAsync_ValidUserId_ReturnsGameState()
        {
            // Arrange
            var userId = "valocity";

            // Act
            var result = await _gameService.StartGameAsync(userId);

            // Assert
            Assert.IsType<GameState>(result);
        }

        /// <summary>
        /// Tests that retrieving a game state with an invalid game ID throws an exception.
        /// </summary>
        [Fact]
        public async Task GetGameStateAsync_InvalidGameId_ThrowsException()
        {
            // Arrange
            var gameId = "invalid-game-id";

            // Assert & Act
            await Assert.ThrowsAsync<Exception>(() => _gameService.GetGameStateAsync(gameId));
        }

        /// <summary>
        /// Tests that retrieving a game state with a valid game ID returns a valid game state.
        /// </summary>
        [Fact]
        public async Task GetGameStateAsync_ValidGameId_ReturnsGameState()
        {
            // Arrange
            var gameId = "game123";

            // Mock
            _gameRepositoryMock.Setup(repo => repo.GetGameStateAsync(gameId))
                .ReturnsAsync(new GameState { });

            // Act
            var result = await _gameService.GetGameStateAsync(gameId);

            // Assert
            Assert.IsType<GameState>(result);
        }

        /// <summary>
        /// Tests submitting a correct sequence with a valid game ID and user sequence.
        /// </summary>
        [Fact]
        public async Task SubmitSequenceAsync_ValidGameId_CorrectSequence_ReturnsGameState()
        {
            // Arrange
            var gameId = "gameID1";

            // Mock
            _gameRepositoryMock.Setup(repo => repo.GetGameStateAsync(gameId))
                .ReturnsAsync(new GameState
                {
                    SequenceHistory = null,
                    CurrentLevel = 1,
                    CorrectSequence = new List<string> { "spades_6" }
                });

            // Act
            var result = await _gameService.SubmitSequenceAsync(gameId, new List<string> { "spades_6" });

            // Assert
            Assert.IsType<GameState>(result);
        }

        /// <summary>
        /// Tests submitting an incorrect sequence with a valid game ID and user sequence throws an exception.
        /// </summary>
        [Fact]
        public async Task SubmitSequenceAsync_ValidGameId_IncorrectSequence_ThrowsException()
        {
            // Arrange
            var gameId = "gameID1";
            _gameRepositoryMock.Setup(repo => repo.GetGameStateAsync(gameId))
                .ReturnsAsync(new GameState
                {
                    SequenceHistory = null,
                    CurrentLevel = 1,
                    CorrectSequence = new List<string> { "spades_6" },
                    SelectedCards = new List<string>()
                });

            // Assert & Act
            await Assert.ThrowsAsync<Exception>(() => _gameService.SubmitSequenceAsync(gameId, new List<string> { "spades_5" }));
        }

        /// <summary>
        /// Tests submitting a sequence with an incorrect round number throws an exception.
        /// </summary>
        [Fact]
        public async Task SubmitSequenceAsync_InvalidRoundNumber_ThrowsException()
        {
            // Arrange
            var gameId = "gameID1";
            _gameRepositoryMock.Setup(repo => repo.GetGameStateAsync(gameId))
                .ReturnsAsync(new GameState
                {
                    SequenceHistory = null,
                    CurrentLevel = 0,
                    CorrectSequence = new List<string> { "spades_6" },
                    SelectedCards = new List<string>()
                });

            // Assert & Act
            await Assert.ThrowsAsync<Exception>(() => _gameService.SubmitSequenceAsync(gameId, new List<string> { "spades_5" }));
        }

        /// <summary>
        /// Tests submitting a sequence with a non-existent game ID throws an exception.
        /// </summary>
        [Fact]
        public async Task SubmitSequenceAsync_NonExistentGameId_ThrowsException()
        {
            // Arrange
            var gameId = "nonexistent-game-id";

            // Corner case when game state is null
            _gameRepositoryMock.Setup(repo => repo.GetGameStateAsync(gameId))
                .ReturnsAsync((GameState)null);

            // Assert & Act
            await Assert.ThrowsAsync<Exception>(() => _gameService.SubmitSequenceAsync(gameId, new List<string> { "spades_6" }));
        }
    }
}
