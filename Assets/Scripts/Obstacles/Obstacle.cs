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
                playerPresentInAnyObstacleTrigger = true;
                GameManager.instance.PlayerDamaged(DamageFactor);
            }
        
            if (other.CompareTag("Bullet"))
            {
                other.transform.parent.gameObject.SetActive(false);
                Destroy();
            }
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
