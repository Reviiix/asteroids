using Obstacles;
using UnityEngine;

namespace PlayArea
{
    public class GameArea : MonoBehaviour
    {
        private const int TimeAfterExitTillObjectDisabled = 1;
        private const int DestructionParticlePrefabPoolIndex = 3;
        public bool horizontalBox;
        
        //The box collider may need to be dynamically sized for unconventional screen sizes that dont conform to the standard 16:9 / 5:3 conventions.
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                other.transform.parent.gameObject.SetActive(false);
                return;
            }
            
            if (other.gameObject.CompareTag("Obstacle"))
            {
                StartCoroutine(GameManager.Wait(TimeAfterExitTillObjectDisabled, () =>
                {
                    other.transform.parent.gameObject.SetActive(false);
                }));
                return;
            }

            if (other.gameObject.CompareTag("Player"))
            {
                if (Obstacle.playerPresentInAnyObstacleTrigger) return;
                
                var screenEdge = ReturnScreenEdgeFromPosition(horizontalBox, other.transform.position);

                GameAreaTransporter.MoveToScreenEdge(other.transform, screenEdge);

                GameManager.DisplayDebugMessage("Player exited via the " + screenEdge + " of the screen, ejecting them on the opposite side.");
            }
        }
        
        public static void CreateDestructionParticle(Vector3 position)
        {
            ObjectPooling.ReturnObjectFromPool(DestructionParticlePrefabPoolIndex, position, Quaternion.identity);
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