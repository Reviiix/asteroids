using UnityEngine;

namespace Obstacles
{
    public class Obstacle : MonoBehaviour
    {
        public const int MaximumAsteroidSize = 2;
        private const int DamageFactor = 1;
        [HideInInspector][Range(0, MaximumAsteroidSize)] 
        public int asteroidSize;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.instance.PlayerDamaged(DamageFactor);
            }
        
            if (other.CompareTag("Bullet"))
            {
                other.transform.parent.gameObject.SetActive(false);
                Destroy();
            }
        }

        public void OnCreation(int startingAsteroidSize)
        {
            asteroidSize = startingAsteroidSize;
        }
        
        private void Destroy()
        {
            asteroidSize--;
            var trans = transform;
            trans.parent.gameObject.SetActive(false);
            GameManager.instance.OnLargerObstacleDestruction(asteroidSize, trans);
        }
    }
}
