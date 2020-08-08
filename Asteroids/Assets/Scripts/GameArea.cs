using UnityEngine;

public class GameArea : MonoBehaviour
{
    //The box collider may need to be dynamically sized for varying screen sizes.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.position.y > 0)
        {
            GameManager.DisplayDebugMessage("Player exited via the top of the screen, ejecting them at the bottom.");
        }
        
        if (other.transform.position.y < 0)
        {
            GameManager.DisplayDebugMessage("Player exited via the bottom of the screen, ejecting them at the top.");
        }

        if (other.transform.position.x > 0)
        {
            GameManager.DisplayDebugMessage("Player exited via the right of the screen, ejecting them at the left.");
        }
        
        if (other.transform.position.x < 0)
        {
            GameManager.DisplayDebugMessage("Player exited via the left of the screen, ejecting them at the right.");
        }
    }
}
