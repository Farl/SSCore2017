#if USE_VISUAL_SCRIPTING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SS
{
    [UnitCategory("SS")]
    [UnitTitle("Stop Audio")]
    [UnitShortTitle("Stop Audio")]
    public class AudioSourceStopUnit : AudioSourceBaseUnit
    {
        protected override void StartWithoutFade(AudioSource audioSource)
        {
            audioSource.Stop();
        }

        protected override IEnumerator FadeCoroutine(AudioSource audioSource, GraphReference graphReference)
        {
            if (audioSource == null)
            {
                RunDone(Flow.New(graphReference));
                yield break;
            }

            var origVolume = audioSource.volume;
            var startVolume = audioSource.volume;
            var startTime = Time.time;
            while (Time.time - startTime < fadeDuration)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, (Time.time - startTime) / fadeDuration);
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = origVolume;
            RunDone(Flow.New(graphReference));
        }
    }
}

#endif