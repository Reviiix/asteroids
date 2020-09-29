using Assets.Scripts;
using UnityEngine;

namespace Obstacles
{
    public class Obstacle : MonoBehaviour
    {
        private SpriteRenderer obstacleRenderer;
        public static bool playerPresentInAnyObstacleTrigger;
        public const int MaximumAsteroidSize = 2;
        private const int DamageFactor = 1;
        [HideInInspector][Range(0, MaximumAsteroidSize)] 
        public int asteroidSize;

        private void Awake()
        {
            Initialise();
        }

        private void Initialise()
        {
            obstacleRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerPresent();
            }
        
            if (other.CompareTag("Bullet"))
            {
                BulletPresent(other.transform.parent.gameObject);
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerPresent();
            }
        }

        private void BulletPresent(GameObject bullet)
        {
            bullet.SetActive(false);
            Destroy();
        }

        private static void PlayerPresent()
        {
            GameManager.instance.OnPlayerCollision(DamageFactor);
        }

        public void OnCreation(int startingAsteroidSize, Sprite newSprite)
        {
            asteroidSize = startingAsteroidSize;
            obstacleRenderer.sprite = newSprite;
        }
        
        private void Destroy()
        {
            asteroidSize--;
            var trans = transform;
            trans.parent.gameObject.SetActive(false);
            GameManager.instance.OnObstacleDestruction(asteroidSize, trans);
        }
    }
}
