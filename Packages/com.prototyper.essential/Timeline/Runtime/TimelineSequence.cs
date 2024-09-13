using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS;
using System.Linq;
using System;


#if USE_TIMELINE
using UnityEngine.Playables;
using UnityEngine.Timeline;
#endif

namespace Diing.Aquila
{
    public class TimelineSequence : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            [NonSerialized] public string name;
            public WrapMode wrapMode = WrapMode.Default;
            public float speedMultiplier = 1.0f;
            public double start;
            public double end;
        }

        public enum SegmentType
        {
            SignalMarker,
            SequenceSegment
        }

        [InspectorButton(new string[] { "Add Data", "Next" }, new string[] { "AddData", "Next" })]
        public bool showDebugInfo = false;
        [SerializeField] private SegmentType segmentType = SegmentType.SignalMarker;
        public List<Data> data = new List<Data>();

        private int playbackDirection = 1;
        public int currentDataIndex = 0;
        public int targetDataIndex = 0;

        public void AddData()
        {
#if USE_TIMELINE
            // Search the PlayableDirector playable signal track to get the start time of each Data
            var playableDirector = GetComponent<PlayableDirector>();
            if (playableDirector == null)
            {
                Debug.LogError("No PlayableDirector found");
                return;
            }
            if (playableDirector.playableAsset == null)
            {
                return;
            }
            var trackAsset = playableDirector.playableAsset as TimelineAsset;
            if (trackAsset == null)
            {
                return;
            }
            double prevTime = 0;
            int index = 0;
            foreach (var track in trackAsset.GetOutputTracks())
            {
                bool setupTrackExist = false;
                switch (segmentType)
                {
                    case SegmentType.SignalMarker:
                        if (track is SignalTrack)
                        {
                            var signalTrack = track as SignalTrack;
                            Debug.Log("SignalTrack found");
                            while (data.Count < signalTrack.GetMarkerCount())
                            {
                                data.Add(new Data());
                            }
                            prevTime = 0.0;
                            index = 0;
                            foreach (var marker in signalTrack.GetMarkers())
                            {
                                data[index].start = prevTime;
                                prevTime = data[index].end = marker.time;
                                index++;
                            }
                            setupTrackExist = true;
                            continue;
                        }
                        break;
                    case SegmentType.SequenceSegment:
                        if (track is PlayableTrack)
                        {
                            var playableTrack = track as PlayableTrack;
                            if (playableTrack.GetClips() != null)
                            {
                                while (data.Count < playableTrack.GetClips().Count())
                                {
                                    data.Add(new Data());
                                }
                                prevTime = 0.0;
                                index = 0;
                                foreach (var clip in playableTrack.GetClips())
                                {
                                    data[index].name = clip.displayName;
                                    data[index].start = clip.start;
                                    data[index].end = clip.end;
                                    var segment = clip.asset as TimelineSequenceSegment;
                                    if (segment != null)
                                    {
                                        data[index].wrapMode = segment.wrapMode;
                                        data[index].speedMultiplier = Mathf.Max(segment.speedMultiplier, 1e-6f);
                                    }
                                    index++;
                                }
                                data.Sort((a, b) => a.start.CompareTo(b.start));
                                setupTrackExist = true;
                            }
                        }
                        break;
                }
                if (setupTrackExist)
                {
                    break;
                }
            }
#else
            Debug.LogError("Timeline is not imported");
#endif
        }
        
        public void Start()
        {
#if USE_TIMELINE
            var playableDirector = GetComponent<PlayableDirector>();
            if (playableDirector == null)
            {
                Debug.LogError("No PlayableDirector found");
                return;
            }
            AddData();

            playableDirector.timeUpdateMode = DirectorUpdateMode.Manual;
            playableDirector.RebuildGraph();
            playableDirector.time = 0;
            if (playableDirector.playOnAwake)
            {
                playableDirector.Play();
            }
#endif
        }

        void Update()
        {
#if USE_TIMELINE
            var playableDirector = GetComponent<PlayableDirector>();
            if (playableDirector == null)
            {
                Debug.LogError("No PlayableDirector found");
                return;
            }
            playableDirector.Evaluate();

            var backupTime = playableDirector.time;
            var backupDataIndex = data.FindIndex((d) => backupTime <= d.end);
            currentDataIndex = backupDataIndex;

            var currentData = data[currentDataIndex];

            // Speed up when target index is larger than current + 1
            float speedScale = 1;
            if (targetDataIndex < currentDataIndex)
            {
                speedScale = 0;
                playableDirector.time = targetDataIndex < 0 ? 0 : data[targetDataIndex].start;
            }
            else if (targetDataIndex > currentDataIndex)
            {
                float timeDiff = (float)(data[targetDataIndex].start - backupTime);
                if (timeDiff > 0)
                {
                    speedScale = (targetDataIndex - currentDataIndex) * ((timeDiff / currentData.speedMultiplier) / 0.5f);
                    speedScale = Mathf.Max(1, speedScale);
                }
            }
            playableDirector.time += playbackDirection * speedScale * Time.deltaTime * currentData.speedMultiplier;

            var nextDataIndex = data.FindIndex((d) => playableDirector.time <= d.end);
            if (backupDataIndex != nextDataIndex)
            {
                if (backupDataIndex >= 0 && (nextDataIndex > targetDataIndex || nextDataIndex < 0))
                {
                    var backupData = data[backupDataIndex];
                    switch (backupData.wrapMode)
                    {
                        case WrapMode.Loop:
                            playableDirector.time = backupData.start;
                            break;
                        default:
                        case WrapMode.Default:
                        case WrapMode.Once: // = Clamp
                        case WrapMode.ClampForever:
                            playableDirector.time = backupData.end - 1e-07;
                            break;
                        case WrapMode.PingPong:
                            playableDirector.time = backupData.end - 1e-07;
                            break;
                    }
                }
            }
#endif
        }

        public void SetTarget(string targetName)
        {
            targetDataIndex = data.FindIndex(
                (d) => d.name.Equals(targetName, StringComparison.OrdinalIgnoreCase)
            );
            if (targetDataIndex >= 0)
            {
                SetTarget(targetDataIndex);
            }
        }

        public void SetTarget(int index)
        {
            targetDataIndex = index;
            if (targetDataIndex < 0 || targetDataIndex >= data.Count)
            {
                targetDataIndex = currentDataIndex;
            }
        }

        public void Next()
        {
            SetTarget(targetDataIndex + 1);
            if (showDebugInfo)
            {
                var targetData = data[targetDataIndex];
                Debug.Log($"{targetData.name} {targetData.start} {targetData.end} x{targetData.speedMultiplier}");
            }
        }
    }
}
