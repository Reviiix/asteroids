using System.Collections;
using UnityEngine;

public static class ShakeObject 
{
    public static bool _shaking;
    private static float _timeRemaining;
    private const float Power = 0.1f;
    private const float InitialDuration = 0.5f;
    private const float Deceleration = 0.75f;

    public static void Initialise()
    {
        _shaking = false;
        _timeRemaining = InitialDuration;
    }
    
    public static void Shake(Transform objectToShake)
    {
        GameManager.instance.StartCoroutine(ShakeRoutine(objectToShake));
    }

    private static IEnumerator ShakeRoutine(Transform objectToShake)
    {
        if (_shaking) yield return null;

        _shaking = true;
        Vector2 startPosition = objectToShake.position;
        
        while (_shaking)
        {
            objectToShake.localPosition = startPosition + (Vector2)Random.insideUnitSphere * Power;
            _timeRemaining -= Time.deltaTime * Deceleration;
            if (_timeRemaining <= 0)
            {
                _shaking = false;
            }
            yield return null;
        }
        
        OnShakeStop();
    }

    private static void OnShakeStop()
    {
        _shaking = false;
        Reset();
    }

    private static void Reset()
    {
        _timeRemaining = InitialDuration;
    }
}
