using System;
using System.Collections;
using Shooting;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerShooting
    {
        public static bool canShoot;
        private const int ObjectPoolIndex = BulletManager.ObjectPoolIndex;
        private static Transform _bulletSpawnLocation;
        private const float TimeBetweenShots = 0.1f;
        private static readonly WaitForSeconds WaitTimeBetweenShots = new WaitForSeconds(TimeBetweenShots);

        public void Initialise()
        {
            _bulletSpawnLocation = GameManager.ReturnPlayer().GetChild(0).transform;
        }

        public void PlayerShootUpdate()
        {
            CheckForInput();
        }

        private static void CheckForInput()
        {
            if (!Input.GetMouseButtonUp(0)) return;
            
            if (!canShoot) return;
            
            canShoot = false;
            
            GameManager.Instance.StartCoroutine(SpawnBullet());
        }

        private static IEnumerator SpawnBullet()
        {
            ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, _bulletSpawnLocation.position, _bulletSpawnLocation.rotation);
            yield return WaitTimeBetweenShots;
            canShoot = true;
        }
    }
}
