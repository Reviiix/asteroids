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
        [SerializeField]
        private Sprite[] obstacleSprites;
        private const int ObjectPoolIndex = 2;
        private static Coroutine _obstacleCreationSequence;
        private const int TimeBetweenSpawningObstacles = 5;
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
            CreateObstacle(Obstacle.MaximumAsteroidSize, obstacleSpawnPositions[Random.Range(0, obstacleSpawnPositions.Length)]);
            StartCreatObstacleSequence();
        }
    
        public void CreateObstacle(int asteroidSize, Transform spawnPosition, bool rotate  =false)
        {
            GameManager.DisplayDebugMessage("Asteroid created. Size: " + asteroidSize + ". Location: " + spawnPosition);
            var v = ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, spawnPosition.position, spawnPosition.rotation);

            SetSize(asteroidSize, v);
            
            v.GetComponentInChildren<Obstacle>().OnCreation(asteroidSize);

            if (rotate)
            {
                v.transform.rotation = Quaternion.Euler(Random.Range(0, 360), -90,-90);
            }
        }

        private void SetSize(int newSize, GameObject gameObjectToReSize)
        {
            switch (newSize)
            {
                case 0:
                    gameObjectToReSize.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    break;
                case 1:
                    gameObjectToReSize.transform.localScale = new Vector3(1, 1, 1);
                    break;
                case 2:
                    gameObjectToReSize.transform.localScale = new Vector3(2, 2, 2);
                    break;
                default:
                    Debug.LogError("That is not an acceptable asteroid size. A size of 0 has been set");
                    SetSize(0, gameObjectToReSize);
                    break;
            }
        }

        public static void MoveObstacles()
        {
            JobSystem.MoveObjectsForward(Obstacles.ToArray(), ObstacleMovementSpeed);
        }
    }
}
