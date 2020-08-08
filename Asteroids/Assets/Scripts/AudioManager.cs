using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private static Vector3 _audioSourceStartingLocation;
    private const int AudioSourcePoolIndex = 0;
    private static readonly HashSet<AudioSource> AudioSources = new HashSet<AudioSource>(); //Definately udse a hashset?

    private void Awake()
    {
        InitialiseVariables();
    }

    private void Start()
    {
        CreateHashSetOfAudioSourcesFromPools();
    }

    private void InitialiseVariables()
    {
        _audioSourceStartingLocation = transform.position;
    }
    
    //This avoids having to use a getcomponent on the object every time you want to play a sound.
    private static void CreateHashSetOfAudioSourcesFromPools()
    {
        for (var i = 0; i < GameManager.Instance.objectPools.pools[AudioSourcePoolIndex].maximumActiveObjects; i++)
        {
            AudioSources.Add(ObjectPooling.ReturnObjectFromPool(0, _audioSourceStartingLocation, Quaternion.identity).GetComponent<AudioSource>());
        }
    }

    public static void PlayClip(AudioClip clip, bool looping = false)
    {
        var audioSource = ReturnFirstUnusedAudioSource();
        audioSource.clip = clip;
        audioSource.loop = looping;
        audioSource.Play();
    }

    private static AudioSource ReturnFirstUnusedAudioSource()
    {
        foreach (var audioSource in AudioSources)
        {
            if (audioSource.isPlaying)
            {
                continue;
            }
            return audioSource;
        }

        Debug.LogWarning("There are not enough audio sources to play that many sounds at once, please set a higher maximum amount in the object pool. The first or default audio source has been returned and may have cut of sounds unexpectedly.");
        return AudioSources.FirstOrDefault();
    }
    
}
