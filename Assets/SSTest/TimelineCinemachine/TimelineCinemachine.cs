namespace SS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using Cinemachine;

    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineCinemachine : MonoBehaviour
    {
        [Button(@"A to B", @"PlayA2B")]
        public PlayableDirector pd;

        [SerializeField] float aTime;
        [SerializeField] float bTime;


        void PlayA2B()
        {
            if (pd)
            {
                aTime = Mathf.Clamp(aTime, 0, (float)pd.duration);
                bTime = Mathf.Clamp(bTime, aTime, (float)pd.duration);
                pd.time = aTime;
                pd.DeferredEvaluate();
            }
        }

        private void Awake()
        {
            pd = GetComponent<PlayableDirector>();

            if (pd)
            {
                // Binding
                var asset = pd.playableAsset;
                if (asset)
                {
                    foreach (var o in asset.outputs)
                    {
                        //Debug.Log($"{o.outputTargetType}{o.streamName}{o.sourceObject}");
                        if (o.outputTargetType == typeof(CinemachineBrain))
                        {
                            var cam = Camera.main;
                            if (cam)
                            {
                                var brain = cam.GetComponent<CinemachineBrain>();
                                if (brain)
                                {
                                    pd.SetGenericBinding(o.sourceObject, brain);
                                }
                            }
                        }
                    }

                    var mTracks = asset.GetType().GetField("m_Tracks", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (mTracks != null)
                    {
                        var tracks = mTracks.GetValue(asset) as IList;
                        foreach (var t in tracks)
                        {
                        }
                    }
                }

                pd.timeUpdateMode = DirectorUpdateMode.Manual;
                pd.Play();
            }
        }

        private void Update()
        {
            if (pd)
            {
                pd.time += Time.deltaTime;
                if (pd.time > bTime)
                {
                    pd.time = aTime;
                }
                //pd.Evaluate();
                pd.DeferredEvaluate();
            }
        }
    }

}