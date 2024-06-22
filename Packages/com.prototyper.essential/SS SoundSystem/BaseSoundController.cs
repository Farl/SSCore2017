using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class BaseSoundController : MonoBehaviour, ISoundController
    {
        [SerializeField] SoundData soundData;

        public void Play(float value)
        {
            if (soundData == null)
                return;
            soundData.PlayOneShot();
        }
    }
}
