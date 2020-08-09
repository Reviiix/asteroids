using TMPro;

namespace Statistics
{
    public static class ScoreTracker
    {
        private static TMP_Text _scoreText;
        private const string ScorePrefix = "SCORE: ";
        private static int _score = 0;
        private static readonly int[] IncrementAmounts = {10, 20, 30};

        public static void Initialise()
        {
            _scoreText = GameManager.instance.userInterfaceManager.scoreText;
        }
        public static void IncrementScore(int amountIndex)
        {
            _score += IncrementAmounts[amountIndex];
            UpdateScoreDisplay();
        }

        private static void UpdateScoreDisplay()
        {
            _scoreText.text = ScorePrefix + _score;
        }
    }
}
