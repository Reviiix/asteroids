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

    [SerializeField] [Header("Pause Menu")]
    private Canvas pauseCanvas;
    public Button pauseButton;
    
    [SerializeField] [Header("Game Over Menu")]
    private Canvas gameOverCanvas;
    public TMP_Text finalTimeText;
    public TMP_Text finalScoreText;

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
    
    private void EnabledAllNonPermanentCanvases(bool state)
    {
        gameOverCanvas.enabled = state;
        pauseCanvas.enabled = state;
    }

}
