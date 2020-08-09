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

        public void OnCreation(Sprite sprite)
        {
            SetSize(asteroidSize);
            _renderer.sprite = sprite;
            _renderer.enabled = true;
        }

        private void SetSize(int newSize)
        {
            switch (newSize)
            {
                case 1:
                    gameObject.transform.localScale = new Vector3(2,2,2);
                    break;
                case 2:
                    gameObject.transform.localScale = new Vector3(5,5,5);
                    break;
                case 3:
                    gameObject.transform.localScale = new Vector3(10,10,10);
                    break;
                default:
                    Debug.LogError("That is not an acceptable asteroid size. A size of 1 has been set");
                    SetSize(1);
                    break;
            }
        }

        private void Destroy()
        {
            if (asteroidSize == 0)
            {
                _renderer.enabled = false;
                gameObject.SetActive(false);
                return;
            }
            GameManager.OnLargerObstacleDestruction(asteroidSize, transform);
        }
    }
}
