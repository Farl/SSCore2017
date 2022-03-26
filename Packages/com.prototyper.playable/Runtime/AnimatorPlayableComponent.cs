/**
 * Playables Graph application - Animator playable component
 **/
namespace SS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Animations;

    [RequireComponent(typeof(Animator))]
    public class AnimatorPlayableComponent : MonoBehaviour
    {
        public PlayableGraph playableGraph;
        private Animator animator;
        private AnimationMixerPlayable weightMixer;

        public enum PlayMode
        {
            CrossFade,
            Once
        }

        public List<AnimationClip> animationClips = new List<AnimationClip>();

        private void Awake()
        {
            weightMixer = SetupWithNewGraph();
        }

        private void OnDestroy()
        {
            if (playableGraph.IsValid())
            {
                playableGraph.Destroy();
            }
        }

        [Range(0.0f, 1.0f)]
        public float slider = 0;

        private void Update()
        {
            if (weightMixer.IsValid() && weightMixer.GetInputWeight(1) != slider)
            {
                weightMixer.SetInputWeight(1, slider);
                weightMixer.SetInputWeight(0, 1-slider);
            }
        }

        [ContextMenu("Setup with original graph")]
        void SetupWithOriginalGraph()
        {
            animator = GetComponent<Animator>();
            playableGraph = animator.playableGraph;
            playableGraph.Stop();

            var playableOutput = playableGraph.GetOutputByType<AnimationPlayableOutput>(0);
            var controllerPlayable = playableOutput.GetSourcePlayable();

            // Mixer
            var mixer = AnimationMixerPlayable.Create(playableGraph, 2, false);
            playableGraph.Connect(controllerPlayable, 0, mixer, 0);

            // Setup output
            playableOutput.SetSourcePlayable(mixer);
        }

        [ContextMenu("Setup with new graph")]
        AnimationMixerPlayable SetupWithNewGraph()
        {
            animator = GetComponent<Animator>();
            //animator.playableGraph.Destroy();

            playableGraph = PlayableGraph.Create(nameof(AnimatorPlayableComponent));
            var playableOutput = AnimationPlayableOutput.Create(playableGraph, $"Animation Output", animator);

            // Animator controller
            var controllerPlayable = AnimatorControllerPlayable.Create(playableGraph, animator.runtimeAnimatorController);

            // Clip
            var clipPlayable = AnimationClipPlayable.Create(playableGraph, animationClips[0]);
            clipPlayable.Play();

            // Mixer
            var mixer = AnimationMixerPlayable.Create(playableGraph, 2, true);
            playableGraph.Connect(controllerPlayable, 0, mixer, 0);
            mixer.SetInputWeight(0, 1);
            playableGraph.Connect(clipPlayable, 0, mixer, 1);
            mixer.SetInputWeight(1, 0);

            // Setup output
            playableOutput.SetSourcePlayable(mixer);

            playableGraph.Play();
            return mixer;
        }

        [ContextMenu("Show Graph")]
        void ShowGraph()
        {
            if (playableGraph.IsValid())
            {
                GraphVisualizerClient.Show(playableGraph);
            }
        }

        // Example code from Unity
        static public AnimatorControllerPlayable PlayAnimatorController(Animator animator, RuntimeAnimatorController controller, out PlayableGraph graph)
        {
            graph = PlayableGraph.Create();
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(graph, "AnimatorControllerPlayable", animator);
            var controllerPlayable = AnimatorControllerPlayable.Create(graph, controller);
            playableOutput.SetSourcePlayable(controllerPlayable);
            //graph.SyncUpdateAndTimeMode(animator);
            graph.Play();

            return controllerPlayable;
        }

        public void Play(string animName, float xfadeTime = 0.3f, PlayMode playMode = PlayMode.CrossFade)
        {
            var clip = animationClips.Find(x => x != null && x.name == animName);

            if (clip == null)
                return;

            if (!weightMixer.IsValid())
                return;

            var coroutine = StartCoroutine(CrossFade(playMode, clip, xfadeTime));
        }

        public void Play(int animIdx, float xfadeTime = 0.3f, PlayMode playMode = PlayMode.CrossFade)
        {
            if (animIdx < 0 || animIdx >= animationClips.Count)
                return;

            var clip = animationClips[animIdx];

            if (clip == null)
                return;

            if (!weightMixer.IsValid())
                return;

            var coroutine = StartCoroutine(CrossFade(playMode, clip, xfadeTime));
        }

        public IEnumerator CrossFade(PlayMode playMode, AnimationClip clip, float xfadeTime)
        {
            var originalFrom = weightMixer;
            var originalTo = weightMixer.GetInput(1);

            // Create new mixer
            var newMixer = AnimationMixerPlayable.Create(playableGraph, inputCount: 2, normalizeWeights: true);
            newMixer.SetInputWeight(0, 1);
            newMixer.SetInputWeight(1, 0);

            // Disconnect first
            originalFrom.DisconnectInput(1);

            playableGraph.Connect(newMixer, 0, originalFrom, 1);
            playableGraph.Connect(originalTo, 0, newMixer, 0);

            // Create clip playable
            var clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
            playableGraph.Connect(clipPlayable, 0, newMixer, 1);

            // Fade in
            var time = xfadeTime;
            while (xfadeTime > 0 && time > 0)
            {
                var weight = time / xfadeTime;

                newMixer.SetInputWeight(0, weight);
                newMixer.SetInputWeight(1, 1 - weight);

                if (animator.updateMode == AnimatorUpdateMode.UnscaledTime)
                    time -= Time.unscaledDeltaTime;
                else
                    time -= Time.deltaTime;

                yield return null;
            }
            newMixer.SetInputWeight(0, 0);
            newMixer.SetInputWeight(1, 1);

            // Play clip
            if (playMode == PlayMode.Once)
            {
                while (clipPlayable.GetTime() < clip.length)
                {
                    yield return null;
                }

                // Fade out
                time = xfadeTime;
                while (xfadeTime > 0 && time > 0)
                {
                    var weight = time / xfadeTime;

                    newMixer.SetInputWeight(0, 1 - weight);
                    newMixer.SetInputWeight(1, weight);

                    if (animator.updateMode == AnimatorUpdateMode.UnscaledTime)
                        time -= Time.unscaledDeltaTime;
                    else
                        time -= Time.deltaTime;

                    yield return null;
                }
                newMixer.SetInputWeight(0, 1);
                newMixer.SetInputWeight(1, 0);
            }

            var currFrom = newMixer.GetOutput(0);
            var currFromIdx = 0;
            for (var i = 0; i < currFrom.GetInputCount(); i++)
            {
                if (currFrom.GetInput(i).Equals(newMixer))
                    currFromIdx = i;
            }
            var currTo = (playMode == PlayMode.CrossFade)?
                newMixer.GetInput(1):
                newMixer.GetInput(0);

            // Disconnect
            currFrom.DisconnectInput(currFromIdx);
            newMixer.DisconnectInput(0);
            newMixer.DisconnectInput(1);

            playableGraph.Connect(currTo, 0, currFrom, currFromIdx);
            newMixer.Destroy();
        }
    }
}