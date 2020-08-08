using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    [SerializeField] [Header("User Interface")]
    private Canvas userInterfaceCanvas;
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public RawImage[] livesDisplay;

    [SerializeField] [Header("Start Menu")]
    private Canvas startCanvas;
    public Button startButton;
    
    [SerializeField] [Header("Pause Menu")]
    private Canvas pauseCanvas;
    public Button pauseButton;
    
    [SerializeField] [Header("Game Over Menu")]
    private Canvas gameOverCanvas;
    public TMP_Text finalTimeText;
    public TMP_Text finalScoreText;

    private void Awake()
    {
        EnableStartCanvas();
    }
    
    private void Start()
    {
        InitialiseVariables();
    }

    private void InitialiseVariables()
    {
        startButton.onClick.AddListener(()=>GameManager.Instance.StartGamePlay());
        startButton.onClick.AddListener(DisableStartCanvas);
    }

    public void UpdateLivesDisplay(bool increase)
    {
        foreach (var life in livesDisplay)
        {
            if (life.enabled == increase)
            {
                continue;
            }

            life.enabled = increase;
        }
    }
    
    private void EnableStartCanvas()
    {
        EnabledAllNonPermanentCanvases(false);
        startCanvas.enabled = true;
        startButton.GetComponent<SequentiallyChangeTextColour>().enabled = true;
    }

    private void DisableStartCanvas()
    {
        EnabledAllNonPermanentCanvases(false);
        startButton.GetComponent<SequentiallyChangeTextColour>().enabled = false;
    }
    
    public void EnableGameOverCanvas()
    {
        EnabledAllNonPermanentCanvases(false);
        gameOverCanvas.enabled = true;
    }
    
    public void EnablePauseCanvas()
    {
        EnabledAllNonPermanentCanvases(false);
        pauseCanvas.enabled = true;
    }
    
    private void EnabledAllNonPermanentCanvases(bool state)
    {
        startCanvas.enabled = state;
        gameOverCanvas.enabled = state;
        pauseCanvas.enabled = state;
    }

}
