using UnityEngine;

namespace Assets.Scripts.Player
{
    public static class PlayerMovement
    {
        private static Camera _mainCamera;
        private static bool _canMove;
        private static bool _canRotate;
        private const float RotationSpeed = 10;
        public const float MovementSpeed = 3;
        #region Mouse And Player Minimum Distance
        private static float _camerasDistanceFromGame;
        private const float MousePlayerMinimumDistance = 0.01f;
        private static float MousePlayerActualMinimumDistance => MousePlayerMinimumDistance + _camerasDistanceFromGame;
        #endregion Mouse And Player Minimum Distance
        private static Transform _player;
        
        public static void Initialise()
        {
            _player = GameManager.ReturnPlayer();
            _mainCamera = GameManager.Instance.mainCamera;
            _camerasDistanceFromGame = -_mainCamera.transform.position.z;
        }

        public static void PlayerMovementUpdate()
        {
            if (_canRotate)
            {
                RotateObjectTowardsPointer(_mainCamera, _player, RotationSpeed);
            }
            if (_canMove)
            {
                MoveObjectTowardsPointer(_mainCamera, _player, MovementSpeed, MousePlayerActualMinimumDistance);
            }
        }

        public static void RestrictPlayerMovement(bool state)
        {
            _canMove = !state;
            _canRotate = !state;
        }
        
        private static void RotateObjectTowardsPointer(Camera camera, Transform transformToRotate, float rotationSpeed)
        {
            var direction = camera.ScreenToWorldPoint(Input.mousePosition) - transformToRotate.position;
            var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            var rotation = Quaternion.AngleAxis(angle, Vector3.back);

            transformToRotate.rotation = Quaternion.Slerp(transformToRotate.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
        
        private static void MoveObjectTowardsPointer(Camera camera, Transform transformToMove, float moveSpeed, float remainingDistance)
        {
            var position = transformToMove.position;
            var targetPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            if  (Vector3.Distance(position, targetPosition) < remainingDistance) return;
        
            targetPosition.z = position.z;
            position = Vector3.MoveTowards(position, targetPosition, moveSpeed * Time.deltaTime);
        
            transformToMove.position = position;
        }
    }
}
