﻿using System.Collections.Generic;
using Assets.Scripts.Player;
using Player;
using UnityEngine;

namespace Assets.Scripts
{
    public static class BulletManager 
    {
        public const int ObjectPoolIndex = 1;
        private static readonly List<GameObject> Bullets = new List<GameObject>();
        #region Bullet Speed
        private const float BaseBulletSpeed = 1f;
        private static float BulletSpeed
        {
            get
            {
                var actualBulletSpeed = BaseBulletSpeed;
                actualBulletSpeed += PlayerMovement.MovementSpeed;
                return actualBulletSpeed;
            }
        }
        #endregion Bullet Speed
        public static bool moveBullets = true;

        public static void Initialise()
        {
            for (var i = 0; i < ObjectPooling.PoolDictionary[ObjectPoolIndex].Count; i++)
            {
                Bullets.Add(ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, Vector3.zero, Quaternion.identity,false));
            }
        }

        public static void MoveBullets()
        {
            if (!moveBullets) return;
            
            JobSystem.MoveObjectsForward(Bullets.ToArray(),BulletSpeed);
        }

        public static void DestroyAllBullets()
        {
            foreach (var bullet in Bullets)
            {
                bullet.SetActive(false);
            }
        }
    }
}