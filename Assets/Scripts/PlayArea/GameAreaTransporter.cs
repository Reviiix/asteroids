using System;
using Player;
using UnityEngine;

namespace PlayArea
{
    public static class GameAreaTransporter 
    {
        public static void MoveToScreenEdge(Transform player, ScreenEdge edge)
        {
            if (ShakeObject._shaking) return;
            
            var position = player.position;
            switch (edge)
            {
                case ScreenEdge.Top:
                case ScreenEdge.Bottom:
                    position  = new Vector3(position.x, -position.y, position.z);
                    player.position = position;
                    break;
                case ScreenEdge.Left:
                case ScreenEdge.Right:
                    position  = new Vector3(-position.x, position.y, position.z);
                    player.position = position;
                    break;
                default:
                    Debug.LogError("There is no screen edge associated with: " + edge + ". Player has been deposited at the bottom of the screen.");
                    MoveToScreenEdge(player, ScreenEdge.Bottom);
                    break;
            }
        }
        
    }
}
