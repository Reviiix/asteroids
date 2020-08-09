using UnityEngine;

namespace Statistics
{
    public static class HighSores
    {
        private const string Key = "AsteroidsHighScore";
    
        public static int ReturnHighScore()
        {
            if (PlayerPrefs.HasKey(Key))
            {
                return PlayerPrefs.GetInt(Key);
            }
            Debug.LogWarning("No previous high score found. This may be the first time the game is played.");
            return 0;
        }

        public static void SetHighScore(int currentScore)
        {
            if (CurrentScoreBeatsHighScore(currentScore))
            {
                PlayerPrefs.SetInt(Key, currentScore);
            }
        }
    
        private static bool CurrentScoreBeatsHighScore(int currentScore)
        {
            return currentScore >= ReturnHighScore();
        }
    }
}
