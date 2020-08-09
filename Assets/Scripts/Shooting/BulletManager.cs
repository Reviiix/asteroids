using System.Collections.Generic;
using Player;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Shooting
{
    public static class BulletManager 
    {
        private static readonly List<GameObject> Bullets = new List<GameObject>();

        #region Bullet Speed
        private static float _bulletSpeed = 0.1f;
        private static float BulletSpeed
        {
            get => _bulletSpeed + PlayerMovement.MovementSpeed;
            set => _bulletSpeed = value;
        }

        #endregion Bullet Speed
        private const int PotentialBulletDistance = 10;
        public const int ObjectPoolIndex = 1;
    
        public static void InitialiseBulletList()
        {
            for (var i = 0; i < ObjectPooling.PoolDictionary[ObjectPoolIndex].Count; i++)
            {
                Bullets.Add(ObjectPooling.ReturnObjectFromPool(ObjectPoolIndex, Vector3.zero, Quaternion.identity,false));
            }
        }
    
        //Uses Job System. //Requires a lot of boiler plate code but is the most optimised development strategy in Unity.
        public static void MoveBullets()
        {
            //Create variables to pass to job.
            var amountOfBullets = Bullets.Count;
            var bulletPositions = new NativeArray<float3>(amountOfBullets, Allocator.TempJob);
            var positionsToMoveTo = new NativeArray<float3>(amountOfBullets, Allocator.TempJob);
            for (var i = 0; i < amountOfBullets; i++)
            {
                if (!Bullets[i].activeInHierarchy) continue;
            
                bulletPositions[i] = Bullets[i].transform.position;

                var pathRay = new Ray(bulletPositions[i], Bullets[i].transform.forward);
                var position = pathRay.GetPoint(PotentialBulletDistance);
                positionsToMoveTo[i]= new float3(position.x, position.y, 0);

                Debug.DrawRay(pathRay.origin, pathRay.direction, Color.green);
            }

            //Create job handle and pass in variables.
            var shootJobHandle = new JobSystem.MoveMultipleObjectsYPosition
            {
                deltaTime = Time.deltaTime,
                positionsOfObjectsToMove = bulletPositions,
                positionOfObjectToMoveTowards = positionsToMoveTo,
                speed = BulletSpeed
            };

            //Run the job with the handle(in batches of 10).
            var handle = shootJobHandle.Schedule(amountOfBullets, 10);
            handle.Complete();
        
            //Reassign the variables (Having had the job applied.) to the bullet positions.
            for (var i = 0; i < amountOfBullets; i++)
            {
                if (Bullets[i].activeInHierarchy)
                {
                    Bullets[i].transform.position = bulletPositions[i];
                }
            }
        
            //Dispose of native arrays (Not handled by garbage collector).
            bulletPositions.Dispose();
            positionsToMoveTo.Dispose();
        }
    }
}
