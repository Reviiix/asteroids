using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerMovement
    {
        private static Camera _mainCamera;
        private static bool _canMove;
        private static bool _canRotate;
        private const float RotationSpeed = 10;
        public const float MovementSpeed = 3;
        #region MousePlayerMinimumDistance
        private static float _camerasDistanceFromGame;
        private float _mousePlayerMinimumDistance = 0.01f;
        private float MousePlayerMinimumDistance
        {
            get => _mousePlayerMinimumDistance + _camerasDistanceFromGame;
            set => _mousePlayerMinimumDistance = value;
        }
        #endregion MousePlayerMinimumDistance
        private Transform _player;
        

        public void Initialise()
        {
            _player = GameManager.ReturnPlayer();
            _mainCamera = GameManager.instance.mainCamera;
            _camerasDistanceFromGame = -_mainCamera.transform.position.z;
        }

        public void PlayerMovementUpdate()
        {
            if (_canRotate)
            {
                RotateObjectTowardsPointer(_mainCamera, _player, RotationSpeed);
            }
            if (_canMove)
            {
                MoveObjectTowardsPointer(_mainCamera, _player, MovementSpeed, MousePlayerMinimumDistance);
            }
        }

        public static void EnablePlayerMovementConstraints(bool state)
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
