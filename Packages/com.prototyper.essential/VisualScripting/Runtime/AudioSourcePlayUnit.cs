#if USE_VISUAL_SCRIPTING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SS
{
    [UnitCategory("SS")]
    [UnitTitle("Play Audio")]
    [UnitShortTitle("Play Audio")]
    public class AudioSourcePlayUnit : AudioSourceBaseUnit
    {
        protected override void StartWithoutFade(AudioSource audioSource)
        {
            audioSource.Play();
        }

        // Fade in
        protected override IEnumerator FadeCoroutine(AudioSource audioSource, GraphReference graphReference)
        {
            if (audioSource == null)
            {
                RunDone(Flow.New(graphReference));
                yield break;
            }

            var origVolume = audioSource.volume;
            audioSource.Play();

            var targetVolume = origVolume;
            var startTime = Time.time;
            while (Time.time - startTime < fadeDuration)
            {
                audioSource.volume = Mathf.Lerp(0, targetVolume, (Time.time - startTime) / fadeDuration);
                yield return null;
            }

            audioSource.volume = origVolume;
            RunDone(Flow.New(graphReference));
        }
    }
}

#endif