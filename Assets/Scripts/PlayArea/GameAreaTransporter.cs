using UnityEngine;

namespace PlayArea
{
    public static class GameAreaTransporter 
    {
        public static void MoveToScreenEdge(Transform transformToMove, ScreenEdge edge)
        {
            var position = transformToMove.position;
            switch (edge)
            {
                case ScreenEdge.Top:
                case ScreenEdge.Bottom:
                    position  = new Vector3(position.x, -position.y, position.z);
                    transformToMove.position = position;
                    break;
                case ScreenEdge.Left:
                case ScreenEdge.Right:
                    position  = new Vector3(-position.x, position.y, position.z);
                    transformToMove.position = position;
                    break;
                default:
                    Debug.LogError("There is no screen edge associated with: " + edge + ". Player has been deposited at the bottom of the screen.");
                    MoveToScreenEdge(transformToMove, ScreenEdge.Bottom);
                    break;
            }
        }
        
        public static void PlaceObjectInCentre(Transform transformToMove)
        {
            transformToMove.position = Vector3.zero;
            transformToMove.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
