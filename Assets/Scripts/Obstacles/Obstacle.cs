using UnityEngine;

namespace Obstacles
{
    public class Obstacle : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        public const int MaximumAsteroidSize = 2;
        private const int DamageFactor = 1;
        [HideInInspector][Range(0, MaximumAsteroidSize)] 
        public int asteroidSize;

        private void Awake()
        {
            InitialiseVariables();
        }

        private void InitialiseVariables()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.PlayerDamaged(DamageFactor);
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
            transform.parent.gameObject.SetActive(false);
            asteroidSize--;
            if (asteroidSize >= 0)
            {
                GameManager.Instance.OnLargerObstacleDestruction(asteroidSize, transform);
            }
        }
    }
}
