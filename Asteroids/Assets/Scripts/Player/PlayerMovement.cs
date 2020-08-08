using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerMovement
    {
        private static Camera _mainCamera;
        public static bool CanMove = false;
        public static bool CanRotate = true;
        private const float RotationSpeed = 10;
        private const float MovementSpeed = 3;
        #region MousePlayerMinimumDistance
        private static float _camerasDistanceFromGame;
        private float _mousePlayerMinimumDistance = 0.1f;
        private float MousePlayerMinimumDistance
        {
            get => _mousePlayerMinimumDistance + _camerasDistanceFromGame;
            set => _mousePlayerMinimumDistance = value;
        }
        #endregion MousePlayerMinimumDistance
        private Transform _player;
        

        public void Initialise() //cant use constructor because singleton game manager isn't set yet.
        {
            _player = GameManager.ReturnPlayer();
            _mainCamera = GameManager.Instance.mainCamera;
            _camerasDistanceFromGame = -_mainCamera.transform.position.z;
        }

        public void PlayerMovementManagerUpdate()
        {
            if (CanRotate)
            {
                RotateObjectTowardsPointer(_mainCamera, _player, RotationSpeed);
            }
            if (CanMove)
            {
                MoveObjectTowardsPointer(_mainCamera, _player, MovementSpeed, MousePlayerMinimumDistance);
            }
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
