using System.Collections;
using UnityEngine;

namespace Obstacles
{
    public class ObstacleManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject obstaclePrefab;
        [SerializeField]
        private Transform[] obstacleSpawnPositions;

        private const int ObstaclePoolIndex = 2;
        private static Coroutine _obstacleCreationSequence;
        private const int TimeBetweenSpawningObstacles = 10;
        private readonly WaitForSeconds _waitTimeBetweenSpawningObstacles = new WaitForSeconds(TimeBetweenSpawningObstacles);

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
            yield return _waitTimeBetweenSpawningObstacles;
            CreateObstacle(Random.Range(0, Obstacle.MaximumAsteroidSize), obstacleSpawnPositions[Random.Range(0, obstacleSpawnPositions.Length)]);
        }
    
        public void CreateObstacle(int asteroidSize, Transform position)
        {
            GameManager.DisplayDebugMessage("Asteroid created. Size: " + asteroidSize + ". Location: " + position);
        }
    }
}
