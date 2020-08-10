using System.Collections;
using Shooting;
using UnityEngine;

namespace Player
{
    public static class PlayerShooting
    {
        public static bool canShoot;
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
            if (!Input.GetMouseButtonUp(0)) return;
            
            if (!canShoot) return;
            
            canShoot = false;
            
            GameManager.instance.StartCoroutine(SpawnBullet());
        }

        private static IEnumerator SpawnBullet()
        {
            GameManager.instance.audioManager.PlayGunShotSound();
            ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, _bulletSpawnLocation.position, _bulletSpawnLocation.rotation);
            yield return WaitTimeBetweenShots;
            canShoot = true;
        }
    }
}
