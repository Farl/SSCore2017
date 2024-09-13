
#if USE_TIMELINE
using UnityEngine;
using UnityEngine.Playables;

namespace SS
{

    [System.Serializable]
    public class TimelineSequenceSegmentBehaviour : PlayableBehaviour
    {
        [System.NonSerialized] public WrapMode wrapMode = WrapMode.Default;
    }
}
#endif