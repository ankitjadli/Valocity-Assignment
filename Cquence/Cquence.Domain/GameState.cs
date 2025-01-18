namespace valocity.assignment.cquence.domain
{
    /// <summary>
    /// Represents the state of a game, including progress, user information, and history of sequences.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// The unique identifier for the game session.
        /// </summary>
        public string GameId { get; set; }

        /// <summary>
        /// The unique identifier of the user playing the game.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The current level the user is on in the game.
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// The correct sequence of cards for the current level.
        /// </summary>
        public List<string> CorrectSequence { get; set; }

        /// <summary>
        /// The list of cards currently selected by the user during gameplay.
        /// </summary>
        public List<string> SelectedCards { get; set; }

        /// <summary>
        /// A history of all correct sequences generated for previous levels.
        /// </summary>
        public List<List<string>> SequenceHistory { get; set; }

        /// <summary>
        /// The last sequence attempt made by the user that was incorrect.
        /// </summary>
        public string IncorrectSequenceAttempt { get; set; }
    }
}
