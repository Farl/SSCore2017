using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    [System.Serializable]
    public class SoundData
    {
        public AudioSource audioSource;
        public float delayTime;
        public float fadeInTime;
        public float fadeOutTime;
        public float playInterval;

        [System.NonSerialized] public Coroutine coroutine;
        [System.NonSerialized] public float origVolume;
        [Range(0f, 1f)] public float fadeMultiplier = 0f;
        [System.NonSerialized] public float lastPlayTime = 0f;

        public float length
        {
            get
            {
                if (audioSource == null)
                    return 0f;
                return audioSource.clip.length;
            }
        }

        public void Stop()
        {
            if (audioSource == null)
                return;
            audioSource.Stop();
        }

        public void PlayOneShot()
        {
            PlaySoundOneShot(this);
        }

        public static void PlaySoundOneShot(SoundData soundData)
        {
            if (soundData.audioSource == null)
                return;
            if (soundData.lastPlayTime + soundData.playInterval > Time.time)
                return;
            soundData.lastPlayTime = Time.time;
            soundData.audioSource.PlayOneShot(
                soundData.audioSource.clip
            );
        }
    }
}
