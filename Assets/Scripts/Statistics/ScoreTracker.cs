using Assets.Scripts;
using TMPro;

namespace Statistics
{
    public static class ScoreTracker
    {
        private static TMP_Text _scoreText;
        private const string ScoreDisplayPrefix = "SCORE: ";
        public static int score;
        private static readonly int[] IncrementAmounts = {5, 10, 20};

        public static void Initialise()
        {
            score = 0;
            _scoreText = GameManager.Instance.userInterfaceManager.scoreText;
            _scoreText.text = ScoreDisplayPrefix + 0;
        }
        public static void IncrementScore(int amountIndex)
        {
            score += IncrementAmounts[amountIndex];
            UpdateScoreDisplay();
        }

        private static void UpdateScoreDisplay()
        {
            _scoreText.text = ScoreDisplayPrefix + score;
        }
    }
}
