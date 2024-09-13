
#if USE_TIMELINE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace SS
{
    public class TimelineSequenceSegment : PlayableAsset
    {
        public Color color = Color.clear;
        public WrapMode wrapMode = WrapMode.Default;
        public float speedMultiplier = 1.0f;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimelineSequenceSegmentBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.wrapMode = wrapMode;
            return playable;
        }
    }
}


#endif