using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Obstacles
{
    [Serializable]
    public class ObstacleManager
    {
        public static bool moveObstacles = true;
        private static readonly List<GameObject> Obstacles = new List<GameObject>();
        private const float ObstacleMovementSpeed = PlayerMovement.MovementSpeed - float.Epsilon;
        [SerializeField]
        private Transform[] obstacleSpawnPositions;
        [SerializeField]
        private Sprite[] obstacleSprites;
        private readonly Vector3[] obstacleSizes = {new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f), new Vector3(2f, 2f, 2f)};
        private const int ObjectPoolIndex = 2;
        private static Coroutine _obstacleCreationSequence;
        private const int TimeBetweenSpawningObstacles = 1;
        private static readonly WaitForSeconds WaitTimeBetweenSpawningObstacles = new WaitForSeconds(TimeBetweenSpawningObstacles);
        private static int _previousSpawnIndex;

        public static void Initialise()
        {
            for (var i = 0; i < ObjectPooling.PoolDictionary[ObjectPoolIndex].Count; i++)
            {
                Obstacles.Add(ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, Vector3.zero, Quaternion.identity,false));
            }
        }

        public void StartCreatObstacleSequence()
        {
            _obstacleCreationSequence = GameManager.instance.StartCoroutine(ContinuouslyCreatObstaclesSequence());
        }
    
        public void StopCreatObstacleSequence()
        {
            if (_obstacleCreationSequence != null)
            {
                GameManager.instance.StopCoroutine(_obstacleCreationSequence);
            }
        }

        private IEnumerator ContinuouslyCreatObstaclesSequence()
        {
            yield return WaitTimeBetweenSpawningObstacles;
            if (Random.Range(0, (int)GameManager.GameDifficulty) != (int)GameManager.GameDifficulty)
            {
                var currentSpawnIndex = ReturnRandomIndexThatIsNotX(_previousSpawnIndex, obstacleSpawnPositions.Length);
                CreateObstacle(Obstacle.MaximumAsteroidSize, obstacleSpawnPositions[currentSpawnIndex]);
                _previousSpawnIndex = currentSpawnIndex;
            }
            StartCreatObstacleSequence();
        }

        private static int ReturnRandomIndexThatIsNotX(int x, int maxValue)
        {
            var returnVariable = Random.Range(0, maxValue);
            
            // ReSharper disable once TailRecursiveCall
            return returnVariable == x ? ReturnRandomIndexThatIsNotX(x, maxValue) : returnVariable;
        }

        public void CreateObstacle(int asteroidSize, Transform spawnPosition, bool randomRotate = false)
        {
            GameManager.DisplayDebugMessage("Asteroid created. Size: " + asteroidSize + ". Location: " + spawnPosition);
            
            var obstacle = ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, spawnPosition.position, spawnPosition.rotation);
            obstacle.transform.localScale = obstacleSizes[asteroidSize];
            obstacle.GetComponentInChildren<Obstacle>().OnCreation(asteroidSize, obstacleSprites[Random.Range(0, obstacleSprites.Length)]);
            if (randomRotate)
            {
                obstacle.transform.rotation = Quaternion.Euler(Random.Range(0, 360), -90,-90);
            }
        }

        public static void MoveObstacles()
        {
            if (!moveObstacles) return;
            
            JobSystem.MoveObjectsForward(Obstacles.ToArray(), ObstacleMovementSpeed);
        }
        
        public static void DestroyAllObstacles()
        {
            foreach (var obstacles in Obstacles)
            {
                obstacles.SetActive(false);
            }
        }
    }
}
