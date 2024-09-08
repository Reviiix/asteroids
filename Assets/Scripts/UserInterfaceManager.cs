using System;
using System.Collections.Generic;
using Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [Serializable]
    public class UserInterfaceManager
    {
        [Header("Player HUD")]
        public TMP_Text timeText;
        public TMP_Text scoreText;
        [SerializeField]
        private RawImage[] livesDisplay;
        [SerializeField]
        private Button pauseButton;
        private Image pauseButtonImage;
        [SerializeField] 
        private Sprite pauseSprite;
        [SerializeField] 
        private Sprite playSprite;

        [SerializeField] [Header("Introduction Menu")]
        private Canvas startCanvas;
        public Button startButton;
    
        [SerializeField] [Header("Pause Menu")]
        private Canvas pauseCanvas;

        [SerializeField] [Header("Game Over Menu")]
        private Canvas gameOverCanvas;
        [SerializeField]
        private TMP_Text finalTimeText;
        [SerializeField]
        private TMP_Text finalScoreText;
        [SerializeField]
        private TMP_Text highScoreText;
        private const string HighScorePrefix = "HIGHSCORE: ";
        private static readonly Color HighScoreColour = Color.green;
        [SerializeField]
        private Button restartButton;
    
        public void Initialise()
        {
            EnableStartCanvas();
            ResolveDependencies();
            AddButtonEvents();
        }

        private void ResolveDependencies()
        {
            pauseButtonImage = pauseButton.GetComponent<Image>();
        }

        private void AddButtonEvents()
        {
            startButton.onClick.AddListener(() => EnableStartCanvas(false));
            startButton.onClick.AddListener(GameManager.Instance.StartGamePlay);
            pauseButton.onClick.AddListener(PauseManager.PauseButtonPressed);
            restartButton.onClick.AddListener(GameManager.Instance.ReloadGame);
            AddButtonClickNoise();
        }

        private void AddButtonClickNoise()
        {
            startButton.onClick.AddListener(GameManager.Instance.audioManager.PlayButtonClickSound);
            restartButton.onClick.AddListener(GameManager.Instance.audioManager.PlayButtonClickSound);
            pauseButton.onClick.AddListener(GameManager.Instance.audioManager.PlayButtonClickSound);
        }

        public void UpdateLivesDisplay(bool increase)
        {
            foreach (var lifeDepletedImage in livesDisplay)
            {
                if (lifeDepletedImage.enabled != increase)
                {
                    continue;
                }
                lifeDepletedImage.enabled = !increase;
                return;
            }
        }

        public void EnableStartCanvas(bool state = true)
        {
            pauseButton.gameObject.SetActive(!state);
        
            EnabledAllNonPermanentCanvases(false);
            startCanvas.enabled = state;
        }

        public void EnableGameOverCanvas(bool state = true)
        {
            pauseButton.gameObject.SetActive(!state);
        
            EnabledAllNonPermanentCanvases(false);
            gameOverCanvas.enabled = state;
            
            finalScoreText.text = scoreText.text;
            finalTimeText.text = timeText.text;
        
            var highScore = HighScores.ReturnHighScore();
            highScoreText.text = HighScorePrefix + highScore;

            if (ScoreTracker.score >= highScore)
            {
                ChangeTextColors(new [] {highScoreText, finalScoreText}, HighScoreColour);
                return;
            }
            ChangeTextColors(new [] {highScoreText, finalScoreText}, Color.white);
        }

        private static void ChangeTextColors(IEnumerable<TMP_Text> textsToChange, Color newColor)
        {
            foreach (var text in textsToChange)
            {
                text.color = newColor;
            }
        }
    
        public void EnablePauseCanvas(bool state = true)
        {
            EnabledAllNonPermanentCanvases(false);
            pauseCanvas.enabled = state;
            pauseButtonImage.sprite = ReturnPauseButtonSprite(PauseManager.isPaused);
        }
    

        public Sprite ReturnPauseButtonSprite(bool state)
        {
            return state ? playSprite : pauseSprite;
        }
    
        private void EnabledAllNonPermanentCanvases(bool state)
        {
            startCanvas.enabled = state;
            gameOverCanvas.enabled = state;
            pauseCanvas.enabled = state;
        }
    }
}
