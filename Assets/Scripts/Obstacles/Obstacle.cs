using UnityEngine;

namespace Obstacles
{
    public class Obstacle : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        public const int MaximumAsteroidSize = 2;
        private const int DamageFactor = 1;
        [Range(0, MaximumAsteroidSize)] 
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
                Destroy();
            }
        }

        private void Initialise(Sprite sprite)
        {
            _renderer.sprite = sprite;
            _renderer.enabled = true;
        }

        private void Destroy()
        {
            if (asteroidSize == 0)
            {
                _renderer.enabled = false;
                gameObject.SetActive(false);
                return;
            }
            GameManager.CreateNewAsteroidFromOld(asteroidSize, transform);
        }
    }
}
