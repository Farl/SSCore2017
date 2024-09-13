using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;

namespace SS
{
    public static class SoundSystem
    {
        [System.Serializable]
        public class AudioData
        {
            public AudioSource audioSource;
            [FormerlySerializedAs("audioID")]
            public string soundEventID;

            [System.NonSerialized]
            public float origVolume;

            [NonSerialized]
            public bool origLoop;
        }

        private static Dictionary<string, AudioData> audioDatas = new Dictionary<string, AudioData>();
        private static Dictionary<string, Coroutine> coroutineMap = new Dictionary<string, Coroutine>();
        private static SoundSystemRunner _Runner;
        private static SoundSystemRunner Runner
        {
            get {
                if (_Runner == null)
                {
                    var go = new GameObject("SoundSystemRunner", typeof(SoundSystemRunner));
                    _Runner = go.GetComponent<SoundSystemRunner>();
                }
                return _Runner;
            }
        }

        public static void Register(AudioData data)
        {
            if (data == null)
            {
                Debug.LogError("AudioData is null");
            }
            else if (data.audioSource == null)
            {
                Debug.LogError("AudioSource is empty");
            }
            else
            {
                // Default name
                if (string.IsNullOrEmpty(data.soundEventID))
                {
                    data.soundEventID = data.audioSource.name;
                }

                if (!audioDatas.TryAdd(data.soundEventID, data))
                {
                    Debug.LogError($"Duplicate audioID = {data.soundEventID}");
                }
                else if (data.audioSource)
                {
                    // Init
                    data.origVolume = data.audioSource.volume;
                    data.origLoop = data.audioSource.loop;
                }
            }
            
        }

        public static void Unregister(AudioData data)
        {
            if (data == null)
            {
                Debug.LogError("AudioData is null");
            }
            else if (audioDatas.ContainsKey(data.soundEventID))
            {
                audioDatas.Remove(data.soundEventID);
            }
            else
            {
                Debug.LogError($"Unexist audioID = {data.soundEventID}");
            }
        }

        public static void PlayOneShot(string audioID)
        {
            if (string.IsNullOrEmpty(audioID))
                return;
            if (audioDatas.TryGetValue(audioID, out var data))
            {
                if (data?.audioSource)
                {
                    data.audioSource.PlayOneShot(data.audioSource.clip);
                }
            }
        }

        public static void Play(string audioID, float fadeInTime = 0)
        {
            if (string.IsNullOrEmpty(audioID))
                return;
            if (audioDatas.TryGetValue(audioID, out var data))
            {
                StopPreviousCoroutine(audioID);
                if (data?.audioSource)
                {
                    if (fadeInTime <= 0)
                    {
                        data.audioSource.volume = data.origVolume;
                        data.audioSource.loop = data.origLoop;
                        data.audioSource.Play();
                    }
                    else
                    {
                        var coroutine = Runner.StartCoroutine(FadeIn(audioID, data, fadeInTime));
                        coroutineMap.Add(audioID, coroutine);
                    }
                }
            }
        }

        public static void Stop(string audioID, float fadeOutTime = 0)
        {
            if (string.IsNullOrEmpty(audioID))
                return;
            if (audioDatas.TryGetValue(audioID, out var data))
            {
                StopPreviousCoroutine(audioID);
                if (data?.audioSource)
                {
                    if (fadeOutTime <= 0)
                    {
                        data.audioSource.Stop();
                    }
                    else
                    {
                        var coroutine = Runner.StartCoroutine(FadeOut(audioID, data, fadeOutTime));
                        coroutineMap.Add(audioID, coroutine);
                    }
                }
            }
        }

        public static float GetEventLength(string audioID)
        {
            if (string.IsNullOrEmpty(audioID))
                return 0;
            if (audioDatas.TryGetValue(audioID, out var data))
            {
                if (data?.audioSource)
                {
                    return data.audioSource.clip.length;
                }
            }
            return 0;
        }

        private static void StopPreviousCoroutine(string audioID)
        {
            if (coroutineMap.TryGetValue(audioID, out var cr))
            {
                Runner.StopCoroutine(cr);
                coroutineMap.Remove(audioID);
            }
        }

        private static IEnumerator FadeOut(string audioID, AudioData data, float fadeOutTime)
        {
            // Disable loop
            data.audioSource.loop = false;

            var startTime = Time.realtimeSinceStartup;
            var time = 0f;
            float startVolume = data.audioSource.volume / data.origVolume;
            float currVolume = startVolume;
            while (true)
            {
                yield return null;
                time = Time.realtimeSinceStartup - startTime;
                currVolume = startVolume * (1f - Mathf.Min(1.0f, time / fadeOutTime));
                data.audioSource.volume = currVolume * data.origVolume;

                if (time >= fadeOutTime)
                    break;
            }
            data.audioSource.Stop();
            StopPreviousCoroutine(audioID);
        }

        private static IEnumerator FadeIn(string audioID, AudioData data, float fadeInTime)
        {
            data.audioSource.volume = 0;
            data.audioSource.loop = data.origLoop;
            data.audioSource.Play();
            var startTime = Time.realtimeSinceStartup;
            var time = 0f;
            float endVolume = data.origVolume;
            float currVolume = 0;
            while (true)
            {
                yield return null;
                time = Time.realtimeSinceStartup - startTime;
                currVolume = (Mathf.Min(1.0f, time / fadeInTime));
                data.audioSource.volume = currVolume * data.origVolume;

                if (time >= fadeInTime)
                    break;
            }
            StopPreviousCoroutine(audioID);
        }
    }
}
