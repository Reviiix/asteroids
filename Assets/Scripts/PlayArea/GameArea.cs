﻿using UnityEngine;

namespace Assets.Scripts.PlayArea
{
    public class GameArea : MonoBehaviour
    {
        private const int TimeAfterExitTillObjectDisabled = 1;
        private const int DestructionParticlePrefabPoolIndex = 3;
        public bool horizontalBox;
        
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

                var screenEdge = ReturnScreenEdgeFromPosition(horizontalBox, other.transform.position);

                GameAreaTransporter.MoveToScreenEdge(other.transform, screenEdge);
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