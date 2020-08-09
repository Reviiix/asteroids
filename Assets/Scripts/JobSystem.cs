using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class JobSystem : MonoBehaviour
{
    [BurstCompile]
    public struct MoveMultipleObjectsYPosition : IJobParallelFor
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


