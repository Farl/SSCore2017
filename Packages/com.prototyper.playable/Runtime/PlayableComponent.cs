/**
 * Playables Graph
 **/
namespace SS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Animations;

    [RequireComponent(typeof(Animator))]
    public class PlayableComponent : MonoBehaviour
    {
        public PlayableGraph playableGraph;
        private Animator animator;
        private AnimationMixerPlayable weightMixer;

        public List<AnimationClip> animationClips = new List<AnimationClip>();

        private void Awake()
        {
            weightMixer = Setup2();
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.playableGraph.Play();
            }

            if (weightMixer.IsValid())
            {
                weightMixer.SetInputWeight(1, slider);
                weightMixer.SetInputWeight(0, 1-slider);
            }
        }

        [ContextMenu("Setup1")]
        void Setup1()
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

        [ContextMenu("Setup2")]
        AnimationMixerPlayable Setup2()
        {
            animator = GetComponent<Animator>();
            //animator.playableGraph.Destroy();

            playableGraph = PlayableGraph.Create(nameof(PlayableComponent));
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

    }

}