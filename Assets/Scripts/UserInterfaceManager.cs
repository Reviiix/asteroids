using System;
using System.Collections.Generic;
using Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UserInterfaceManager
{
    [SerializeField] [Header("Player HUD")]
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public RawImage[] livesDisplay;
    public Button pauseButton;
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
    public TMP_Text finalTimeText;
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;
    private const string HighScorePrefix = "HIGHSCORE: ";
    private static readonly Color HighScoreColour = Color.green;
    public Button restartButton;
    
    public void Initialise()
    {
        EnableStartCanvas();
        InitialiseVariables();
        AddButtonEvents();
    }

    private void InitialiseVariables()
    {
        pauseButtonImage = pauseButton.GetComponent<Image>();
    }

    private void AddButtonEvents()
    {
        startButton.onClick.AddListener(() => EnableStartCanvas(false));
        startButton.onClick.AddListener(GameManager.instance.StartGamePlay);
        pauseButton.onClick.AddListener(PauseManager.PauseGamePlay);
        restartButton.onClick.AddListener(GameManager.instance.ReloadGame);
        AddButtonClickNoise();
    }

    private void AddButtonClickNoise()
    {
        startButton.onClick.AddListener(GameManager.instance.audioManager.PlayButtonClickSound);
        restartButton.onClick.AddListener(GameManager.instance.audioManager.PlayButtonClickSound);
        pauseButton.onClick.AddListener(GameManager.instance.audioManager.PlayButtonClickSound);
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
        SequentiallyChangeTextColour.StartChangeTextColorSequence(startButton.GetComponent<TMP_Text>());
    }

    public void EnableGameOverCanvas(bool state = true)
    {
        pauseButton.gameObject.SetActive(!state);
        
        EnabledAllNonPermanentCanvases(false);
        gameOverCanvas.enabled = state;
        
        SequentiallyChangeTextColour.StartChangeTextColorSequence(restartButton.GetComponent<TMP_Text>());
        
        finalScoreText.text = scoreText.text;
        finalTimeText.text = timeText.text;
        
        var highScore = HighSores.ReturnHighScore();
        highScoreText.text = HighScorePrefix + highScore;

        if (ScoreTracker.score >= highScore)
        {
            ChangeTextColors(new [] {highScoreText, finalScoreText}, HighScoreColour);
        }
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
        
        SequentiallyChangeTextColour.StopChangeTextColorSequence();
    }
}
