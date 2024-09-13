using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class Soundbank : MonoBehaviour
    {
        public List<SoundSystem.AudioData> audioDatas = new List<SoundSystem.AudioData>();

        private void Awake()
        {
            foreach (var a in audioDatas)
                SoundSystem.Register(a);
        }

        private void OnDestroy()
        {
            foreach (var a in audioDatas)
                SoundSystem.Unregister(a);
        }
    }
}
