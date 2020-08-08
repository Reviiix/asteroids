using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace PlayArea
{
    public class GameArea : MonoBehaviour
    {
        public bool horizontalBox;
        
        //The box collider may need to be dynamically sized for varying screen sizes.
        private void OnTriggerExit2D(Collider2D other)
        {
            var screenEdge = ReturnScreenEdgeFromPosition(horizontalBox, other.transform.position);
            
            GameAreaTransporter.MoveToScreenEdge(other.transform, screenEdge);
            
            GameManager.DisplayDebugMessage("Player exited via the " + screenEdge +" of the screen, ejecting them on the opposite side.");
        }

        private static ScreenEdge ReturnScreenEdgeFromPosition(bool horizontal, Vector3 position)
        {
            if (horizontal)
            {
                if (position.x > 0)
                {
                    return ScreenEdge.Right;
                }
                if (position.x < 0)
                {
                    return ScreenEdge.Left;
                }
            }
            else
            {
                if (position.y > 0)
                {
                    return ScreenEdge.Top;
                }
                if (position.y < 0)
                {
                    return ScreenEdge.Bottom;
                }
            }
            Debug.LogError("There is no side corresponding to that position. This is impossible and probably a null position parameter. The bottom side has been returned to maintain the code flow.");
            return ScreenEdge.Bottom;
        }
    }

    public enum ScreenEdge
    {
        Top, Bottom, Left, Right
    }
}