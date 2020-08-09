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
    private const string HighScorePrefix = "HIGHSCORE: ";
    private static readonly Color HighScoreColour = Color.blue;
    
    
    public void Initialise()
    {
        EnableStartCanvas();
        InitialiseVariables();
    }

    private void InitialiseVariables()
    {
        startButton.onClick.AddListener(()=>GameManager.instance.StartGamePlay());
        startButton.onClick.AddListener(()=>EnableStartCanvas(false));
        pauseButton.onClick.AddListener(()=>PauseManager.PauseGamePlay());
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

    private void EnableStartCanvas(bool state = true)
    {
        EnabledAllNonPermanentCanvases(false);
        startCanvas.enabled = state;
        startButton.GetComponent<SequentiallyChangeTextColour>().enabled = state;
    }

    public void EnableGameOverCanvas()
    {
        EnabledAllNonPermanentCanvases(false);
        gameOverCanvas.enabled = true;
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
