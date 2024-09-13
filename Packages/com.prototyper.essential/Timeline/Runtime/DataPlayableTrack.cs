using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SS;

//[System.Serializable]
public class DataPlayableTrack<T, U> : PlayableTrack where T : InterpolationData<T>, new() where U : DataPlayableBehaviour<T, U>, new()
{
    [SerializeField] protected bool showDebugInfo = false;
    
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var pd = go.GetComponent<PlayableDirector>();
        var binding = pd.GetGenericBinding(this) as IPlayableDataModify<T>;
        var clips = GetClips();
        foreach (var c in clips)
        {
            var behaviour = ((DataPlayableAsset<U>)c.asset).template;
            // Inject the binding
            behaviour.binding = binding;
            // Set the start and end time from the clip
            behaviour.start = c.start;
            behaviour.end = c.end;
        }
        return ScriptPlayable<U>.Create(graph, inputCount);
    }
}
