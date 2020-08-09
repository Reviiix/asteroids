using System.Collections;
using UnityEngine;

namespace Obstacles
{
    public class ObstacleManager : MonoBehaviour
    {
        [SerializeField]
        private Transform[] obstacleSpawnPositions;

        private const int ObjectPoolIndex = 2;
        private static Coroutine _obstacleCreationSequence;
        private const int TimeBetweenSpawningObstacles = 10;
        private static readonly WaitForSeconds WaitTimeBetweenSpawningObstacles = new WaitForSeconds(TimeBetweenSpawningObstacles);

        public void StartCreatObstacleSequence()
        {
            _obstacleCreationSequence = StartCoroutine(CreatObstacles());
        }
    
        public void StopCreatObstacleSequence()
        {
            if (_obstacleCreationSequence != null)
            {
                StopCoroutine(_obstacleCreationSequence);
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
            //ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, spawnPosition.position, Quaternion.identity);
        }
    }
}
