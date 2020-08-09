using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public static class JobSystem 
{
    //The Job system requires a lot of boiler plate code but is the most optimised development strategy in Unity.
    public static void MoveObjectsForward(GameObject[] objectsToMove, float speed)
    {
        //Create variables to pass to job.
        var amountOfObjects = objectsToMove.Length;
        var objectPositions = new NativeArray<float3>(amountOfObjects, Allocator.TempJob);
        var positionsToMoveTo = new NativeArray<float3>(amountOfObjects, Allocator.TempJob);
        for (var i = 0; i < amountOfObjects; i++)
        {
            if (!objectsToMove[i].activeInHierarchy) continue;
        
            objectPositions[i] = objectsToMove[i].transform.position;

            var pathRay = new Ray(objectPositions[i], objectsToMove[i].transform.forward);
            var position = pathRay.GetPoint(1);
            positionsToMoveTo[i]= new float3(position.x, position.y, 0);

            Debug.DrawRay(pathRay.origin, pathRay.direction, Color.green);
        }

        //Create job handle and pass in variables.
        var shootJobHandle = new JobSystem.MoveMultipleObjectsTowardsPosition
        {
            deltaTime = Time.deltaTime,
            positionsOfObjectsToMove = objectPositions,
            positionOfObjectToMoveTowards = positionsToMoveTo,
            speed = speed
        };

        //Run the job with the handle(in batches of 10).
        var handle = shootJobHandle.Schedule(amountOfObjects, 10);
        handle.Complete();
    
        //Reassign the variables (Having had the job applied.) to the bullet positions.
        for (var i = 0; i < amountOfObjects; i++)
        {
            if (objectsToMove[i].activeInHierarchy)
            {
                objectsToMove[i].transform.position = objectPositions[i];
            }
        }
    
        //Dispose of native arrays (Not handled by garbage collector).
        objectPositions.Dispose();
        positionsToMoveTo.Dispose();
    }
        
    [BurstCompile]
    private struct MoveMultipleObjectsTowardsPosition : IJobParallelFor
    {
        public NativeArray<float3> positionsOfObjectsToMove;
        public NativeArray<float3> positionOfObjectToMoveTowards;
        public float speed;
        [ReadOnly]
        public float deltaTime;

        public void Execute(int index)
        {
            var directionOfTravel = positionOfObjectToMoveTowards[index] - positionsOfObjectsToMove[index];
            positionsOfObjectsToMove[index] += new float3(directionOfTravel.x * speed * deltaTime, directionOfTravel.y * speed * deltaTime, directionOfTravel.z * speed * deltaTime);
        }
    }
}


