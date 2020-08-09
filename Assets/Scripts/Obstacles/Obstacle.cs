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
                other.GetComponent<BoxCollider2D>().enabled = false;
            }
        
            if (other.CompareTag("Bullet"))
            {
                other.transform.parent.gameObject.SetActive(false);
                Destroy();
            }
            
            if (other.CompareTag("Obstacle"))
            {
                transform.parent.rotation = Quaternion.Euler(UnityEngine.Random.Range(0, 360), -90,-90);
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
