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
        startButton.onClick.AddListener(()=>GameManager.instance.StartGamePlay());
        startButton.onClick.AddListener(()=>EnableStartCanvas(false));
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

    public void UpdateScoreDisplay()
    {
        
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
