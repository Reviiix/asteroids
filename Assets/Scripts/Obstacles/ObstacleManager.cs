using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Obstacles
{
    [Serializable]
    public class ObstacleManager
    {
        private static readonly List<GameObject> Obstacles = new List<GameObject>();
        private const float ObstacleMovementSpeed = 1f;
        [SerializeField]
        private Transform[] obstacleSpawnPositions;
        private const int ObjectPoolIndex = 2;
        private static Coroutine _obstacleCreationSequence;
        private const int TimeBetweenSpawningObstacles = 1;
        private static readonly WaitForSeconds WaitTimeBetweenSpawningObstacles = new WaitForSeconds(TimeBetweenSpawningObstacles);

        private void OnDisable()
        {
            StopCreatObstacleSequence();
        }
        
        public static void InitialiseObstacleList()
        {
            for (var i = 0; i < ObjectPooling.PoolDictionary[ObjectPoolIndex].Count; i++)
            {
                Obstacles.Add(ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, Vector3.zero, Quaternion.identity,false));
            }
        }

        public void StartCreatObstacleSequence()
        {
            _obstacleCreationSequence = GameManager.Instance.StartCoroutine(CreatObstacles());
        }
    
        public void StopCreatObstacleSequence()
        {
            if (_obstacleCreationSequence != null)
            {
                GameManager.Instance.StopCoroutine(_obstacleCreationSequence);
            }
        }

        private IEnumerator CreatObstacles()
        {
            yield return WaitTimeBetweenSpawningObstacles;
            CreateObstacle(Random.Range(0, Obstacle.MaximumAsteroidSize), obstacleSpawnPositions[Random.Range(0, obstacleSpawnPositions.Length)]);
        }
    
        public static void CreateObstacle(int asteroidSize, Transform spawnPosition)
        {
            GameManager.DisplayDebugMessage("Asteroid created. Size: " + asteroidSize + ". Location: " + spawnPosition);
            ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, spawnPosition.position, spawnPosition.rotation);
        }
        
        //Uses Job System. //Requires a lot of boiler plate code but is the most optimised development strategy in Unity.
        public static void MoveObstacles()
        {
            JobSystem.MoveObjectsForward(Obstacles.ToArray(), ObstacleMovementSpeed);
        }
    }
}
