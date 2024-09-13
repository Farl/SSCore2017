using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace SS
{
    public interface IPlayableDataModify<T>
    {
        void SetOverrideValue(T value, bool behaviourExist);
        void OnGraphStart();
        void OnGraphStop();

    }

    public class InterpolationData<T>
    {
        public InterpolationData()
        {
        }

        public virtual void AddValueWithWeight(T value, float weight)
        {
        }

        public virtual void Lerp(T a, T b, float t)
        {
        }
    }

    [System.Serializable]
    public class DataPlayableBehaviour<T, U> : PlayableBehaviour where T : InterpolationData<T>, new() where U : DataPlayableBehaviour<T, U>, new()
    {
        public T valueStart;
        public T valueEnd;

        [System.NonSerialized] public IPlayableDataModify<T> binding;
        [System.NonSerialized] public double start;
        [System.NonSerialized] public double end;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            if (binding != null)
            {
                binding.OnGraphStart();
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (binding != null)
            {
                binding.OnGraphStop();
            }
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
            ProcessFrameInternal(playable, info, playerData);
        }

        private static void ProcessFrameInternal(Playable playable, FrameData info, object playerData)
        {
            var data = playerData as IPlayableDataModify<T>;
            var time = playable.GetTime();
            int behaviourCount = 0;

            T value = new T();
            T prevEndValue = new T();
            T nextStartValue = new T();

            double nextStart = -1;
            double prevEnd = -1;

            //get the number of all clips on this track
            int inputCount = playable.GetInputCount();
            for (var i = 0; i < inputCount; i++)
            {
                //get the clip
                var inputPlayable = (ScriptPlayable<U>)playable.GetInput(i);
                if (inputPlayable.IsValid())
                {
                    // weight
                    var inputWeight = playable.GetInputWeight(i);

                    // get the clip's script
                    var inputBehaviour = inputPlayable.GetBehaviour();
                    if (inputBehaviour != null)
                    {
                        float lerpTime = (float)((time - inputBehaviour.start) / inputPlayable.GetDuration());

                        // Call T Lerp
                        T clipValueVec = new T();
                        clipValueVec.Lerp(inputBehaviour.valueStart as T, inputBehaviour.valueEnd as T, lerpTime);

                        // Test and set nextStart, prevEnd
                        if (inputBehaviour.start >= time && (inputBehaviour.start < nextStart || nextStart < 0))
                        {
                            // The most closest start time after current time
                            nextStart = inputBehaviour.start;
                            nextStartValue = clipValueVec;
                        }
                        if (inputBehaviour.end <= time && (inputBehaviour.end > prevEnd || prevEnd < 0))
                        {
                            // The most closest end time before current time
                            prevEnd = inputBehaviour.end;
                            prevEndValue = clipValueVec;
                        }

                        if (inputWeight > 0 && (time >= inputBehaviour.start || time <= inputBehaviour.end))
                        {
                            value.AddValueWithWeight(clipValueVec, inputWeight);
                            behaviourCount++;
                        }
                        else
                        {
                            // if out of start end, skip
                        }
                    }
                }
            }

            if (behaviourCount <= 0)
            {
                // Calculate from prevEnd and nextStart
                if (prevEnd >= 0 && time >= prevEnd)
                {
                    value = prevEndValue;
                }
                else if (nextStart >= 0 && time <= nextStart)
                {
                    value = nextStartValue;
                }
            }

            if (data != null)
            {
                data.SetOverrideValue(value, behaviourCount > 0);
            }
        }
    }
}
