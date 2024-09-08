using UnityEngine;

namespace Assets.Scripts.Obstacles
{
    public class Obstacle : MonoBehaviour
    {
        private SpriteRenderer obstacleRenderer;
        public const int MaximumAsteroidSize = 2;
        private const int DamageFactor = 1;
        private int _asteroidSize;

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
            DestroySelf();
        }

        private static void PlayerPresent()
        {
            GameManager.Instance.OnPlayerCollision(DamageFactor);
        }

        public void OnCreation(int startingAsteroidSize, Sprite newSprite)
        {
            _asteroidSize = startingAsteroidSize;
            obstacleRenderer.sprite = newSprite;
        }
        
        private void DestroySelf()
        {
            _asteroidSize--;
            var trans = transform;
            trans.parent.gameObject.SetActive(false);
            GameManager.Instance.OnObstacleDestruction(_asteroidSize, trans);
        }
    }
}
