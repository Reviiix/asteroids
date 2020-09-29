using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public static class PlayerShooting
    {
        public static bool canShoot = false;
        private static bool _shootDelay = true;
        private const int ObjectPoolIndex = BulletManager.ObjectPoolIndex;
        private static Transform _bulletSpawnLocation;
        private const float TimeBetweenShots = 0.3f;
        private static readonly WaitForSeconds WaitTimeBetweenShots = new WaitForSeconds(TimeBetweenShots);

        public static void Initialise()
        {
            _bulletSpawnLocation = GameManager.ReturnPlayer().GetChild(0).transform;
        }

        public static void PlayerShootUpdate()
        {
            CheckForInput();
        }

        private static void CheckForInput()
        {
            if (!canShoot) return;
            
            if (!Input.GetMouseButtonUp(0)) return;
            
            if (!_shootDelay) return;
            
            _shootDelay = false;
            
            GameManager.instance.StartCoroutine(SpawnBullet());
        }

        private static IEnumerator SpawnBullet()
        {
            GameManager.instance.audioManager.PlayGunShotSound();
            ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, _bulletSpawnLocation.position, _bulletSpawnLocation.rotation);
            yield return WaitTimeBetweenShots;
            _shootDelay = true;
        }
    }
}
