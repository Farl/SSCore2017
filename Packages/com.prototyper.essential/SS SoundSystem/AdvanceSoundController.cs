using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class AdvanceSoundController : MonoBehaviour, ISoundController
    {
        [SerializeField] bool showDebugInfo;
        [SerializeField] SoundData startSound;
        [SerializeField] SoundData[] loopSounds;
        [SerializeField] SoundData endSound;

        [Range(-1f, 1f)] public float currValue = -1f;
        private float prevValue = -1;

        void Awake()
        {
            foreach (var sound in loopSounds)
            {
                sound.origVolume = sound.audioSource.volume;
            }
        }

        public void Play(float value)
        {
            currValue = value;
        }

        private void Update()
        {
            {
                if (loopSounds.Length == 0)
                    return;
                if (loopSounds.Length == 1)
                {
                    var sound = loopSounds[0];
                    sound.audioSource.volume = sound.fadeMultiplier * sound.origVolume;
                }
                else
                {
                    // Modify volume
                    var value = Mathf.Clamp01(currValue) * (loopSounds.Length - 1);
                    var index = Mathf.FloorToInt(value);
                    var normalizedValue = value - index;

                    if (showDebugInfo)
                        Debug.Log($"{value} = {index} + {normalizedValue}");

                    for (var i = 0; i < loopSounds.Length; i++)
                    {
                        var sound = loopSounds[i];
                        if (sound.audioSource == null || !sound.audioSource.isPlaying)
                            continue;

                        var volume = 0f;
                        if (index + 1 > loopSounds.Length && i == index)
                            volume = 1f;
                        else if (i == index)
                            volume = (1f - normalizedValue);
                        else if (i == index + 1)
                            volume = normalizedValue;
                        else
                            volume = 0f;

                        sound.audioSource.volume = volume * sound.origVolume * sound.fadeMultiplier;
                    }
                }
            }

            if (prevValue == currValue)
                return;

            if (prevValue < 0 && currValue >= 0)
            {
                // Start
                startSound.PlayOneShot();

                foreach (var sound in loopSounds)
                {
                    PlaySoundLoop(sound);
                }
            }
            else if (prevValue >= 0 && currValue < 0)
            {
                // Stop
                endSound.PlayOneShot();

                foreach (var sound in loopSounds)
                {
                    StopSoundLoop(sound);
                }
            }
            prevValue = currValue;
        }

        void PlaySoundLoop(SoundData soundData)
        {
            if (soundData.audioSource == null)
                return;
            soundData.audioSource.loop = true;
            soundData.audioSource.volume = 0f;
            if (soundData.coroutine != null)
            {
                StopCoroutine(soundData.coroutine);
                soundData.coroutine = null;
            }
            soundData.coroutine = StartCoroutine(FadeInMultiplierCoroutine(soundData));
        }

        IEnumerator FadeInMultiplierCoroutine(SoundData sound)
        {
            if (!sound.audioSource.isPlaying)
                sound.audioSource.Play();
            // Delay
            var time = 0f;
            while (time < sound.delayTime)
            {
                yield return null;
                time += Time.deltaTime;
            }
            // Fade in
            time = 0f;
            while (sound.fadeInTime > 0 && sound.fadeMultiplier < 1f)
            {
                sound.fadeMultiplier = Mathf.MoveTowards(sound.fadeMultiplier, 1f, Time.deltaTime / sound.fadeInTime);
                yield return null;
                time += Time.deltaTime;
            }
            sound.fadeMultiplier = 1f;
            sound.coroutine = null;
        }

        IEnumerator FadeOutMultiplierCoroutine(SoundData sound)
        {
            var time = 0f;
            while (sound.fadeOutTime > 0 && sound.fadeMultiplier > 0f)
            {
                sound.fadeMultiplier = Mathf.MoveTowards(sound.fadeMultiplier, 0f, Time.deltaTime / sound.fadeOutTime);
                yield return null;
                time += Time.deltaTime;
            }
            sound.fadeMultiplier = 0f;
            sound.coroutine = null;
        }

        void StopSoundLoop(SoundData soundData)
        {
            if (soundData.audioSource == null)
                return;
            if (soundData.coroutine != null)
            {
                StopCoroutine(soundData.coroutine);
                soundData.coroutine = null;
            }
            soundData.coroutine = StartCoroutine(FadeOutMultiplierCoroutine(soundData));
        }

        IEnumerator PlaySoundFadeInCoroutine(SoundData soundData)
        {
            // Start
            soundData.audioSource.Play();

            var time = 0f;
            if (soundData.fadeInTime <= 0)
            {
                soundData.audioSource.volume = 1f;
            }
            else
            {
                // FadeIn
                soundData.audioSource.volume = 0f;
                time = 0f;
                while (time < soundData.fadeInTime)
                {
                    soundData.audioSource.volume = time / soundData.fadeInTime;
                    yield return null;
                    time += Time.deltaTime;
                }
                soundData.audioSource.volume = 1f;
            }
            soundData.coroutine = null;
        }

        void PlaySound(SoundData soundData)
        {
            if (soundData.coroutine != null)
            {
                StopCoroutine(soundData.coroutine);
                soundData.coroutine = null;
            }
            soundData.coroutine = StartCoroutine(PlaySoundCoroutine(soundData));
        }

        IEnumerator PlaySoundFadeOutCoroutine(SoundData soundData)
        {
            var clip = soundData.audioSource.clip;
            var length = clip.length;
            var time = 0f;

            // Fade out
            if (soundData.fadeOutTime <= 0)
            {
                soundData.audioSource.volume = 0f;
            }
            else
            {
                soundData.audioSource.volume = 1f;
                while (time < soundData.fadeOutTime)
                {
                    soundData.audioSource.volume = 1f - time / soundData.fadeOutTime;
                    yield return null;
                    time += Time.deltaTime;
                }
                soundData.audioSource.volume = 0f;
            }
            soundData.audioSource.Stop();
        }

        IEnumerator PlaySoundCoroutine(SoundData soundData)
        {
            yield return PlaySoundFadeInCoroutine(soundData);

            // Play until should start fade out
            var clip = soundData.audioSource.clip;
            var length = clip.length;
            var time = 0f;
            while (time < length - soundData.fadeOutTime)
            {
                yield return null;
                time += Time.deltaTime;
            }
            yield return PlaySoundFadeOutCoroutine(soundData);

            soundData.coroutine = null;
        }

    }
}