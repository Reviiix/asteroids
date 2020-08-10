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
    private const string HighScorePrefix = "<u>HIGHSCORE: </u>";
    private static readonly Color HighScoreColour = Color.blue;
    public Button restartButton;
    
    
    public void Initialise()
    {
        EnableStartCanvas();
        InitialiseVariables();
    }

    private void InitialiseVariables()
    {
        startButton.onClick.AddListener(()=>EnableStartCanvas(false));
        startButton.onClick.AddListener(GameManager.instance.StartGamePlay);
        pauseButton.onClick.AddListener(PauseManager.PauseGamePlay);
        restartButton.onClick.AddListener(GameManager.instance.ReloadGame);
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
        EnabledAllNonPermanentCanvases(false);
        startCanvas.enabled = state;
        startButton.GetComponent<SequentiallyChangeTextColour>().enabled = state;
    }

    public void EnableGameOverCanvas(bool state = true)
    {
        EnabledAllNonPermanentCanvases(false);
        gameOverCanvas.enabled = state;
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
    }
    
    private void EnabledAllNonPermanentCanvases(bool state)
    {
        startCanvas.enabled = state;
        gameOverCanvas.enabled = state;
        pauseCanvas.enabled = state;
    }
}
