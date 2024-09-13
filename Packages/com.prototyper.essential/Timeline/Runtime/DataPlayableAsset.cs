using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace SS
{
    public class DataPlayableAsset<T> : PlayableAsset where T : class, IPlayableBehaviour, new()
    {
        public T template;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<T>.Create(graph, template, inputCount: 0);
            return playable;
        }
    }
}