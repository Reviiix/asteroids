using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class AudioManager 
    {
        private static readonly Vector3 AudioSourceStartingLocation = Vector3.zero;
        private const int AudioSourcePoolIndex = 0;
        private static readonly HashSet<AudioSource> AudioSources = new HashSet<AudioSource>();
        private const float NormalAudioVolume = 1;
        private const float PauseAudioVolume = 0.2f;
        [SerializeField]
        private AudioClip buttonClick;
        [SerializeField]
        private AudioClip shooting;
        [SerializeField]
        private AudioClip destruction;
        [SerializeField]
        private AudioClip damage;

        public void PlayGunShotSound()
        {
            PlayClip(shooting);
        }
    
        public void PlayDestructionSound()
        {
            PlayClip(destruction);
        }
    
        public void PlayDamageSound()
        {
            PlayClip(damage);
        }
    
        public void PlayButtonClickSound()
        {
            PlayClip(buttonClick);
        }
    
        public static void Initialise()
        {
            for (var i = 0; i < GameManager.instance.objectPools.pools[AudioSourcePoolIndex].maximumActiveObjects; i++)
            {
                AudioSources.Add(ObjectPooling.ReturnObjectFromPool(0, AudioSourceStartingLocation, Quaternion.identity).GetComponent<AudioSource>());
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
        
        public static void SetPauseVolume(bool pause)
        {
            SetGlobalVolume(pause ? PauseAudioVolume : NormalAudioVolume);
        }

        public static void SetGlobalVolume(float volume)
        {
            AudioListener.volume = volume;
        }
    
    }
}
