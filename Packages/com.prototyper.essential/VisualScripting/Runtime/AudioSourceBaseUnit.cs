// Visual scripting node(Bolt) for control an audio source.


#if USE_VISUAL_SCRIPTING
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

namespace SS
{
    // Hidden unit
    [UnitCategory("SS/Hidden")]
    [UnitTitle("Audio Source Base")]
    public class AudioSourceBaseUnit : Unit, IGraphElementWithData
    {
        public class Data : IGraphElementData
        {
            public HashSet<Flow> activeCoroutines = new HashSet<Flow>();
        }

        [DoNotSerialize, PortLabelHidden] public ControlInput input;

        [DoNotSerialize, PortLabelHidden] public ValueInput audioSourceInput;

        [UnitHeaderInspectable("Duration")] public float fadeDuration = 1f;

        [DoNotSerialize, PortLabelHidden] public ControlOutput output;
        [DoNotSerialize, PortLabel("Done")] public ControlOutput done;

        /// <summary>
        /// Run this event in a coroutine, enabling asynchronous flow like wait nodes.
        /// </summary>
        [Serialize]
        [Inspectable]
        [InspectorExpandTooltip]
        public bool coroutine { get; set; } = false;

        protected override void Definition()
        {
            input = ControlInput(nameof(input), OnInput);
            output = ControlOutput(nameof(output));
            done = ControlOutput(nameof(done));

            audioSourceInput = ValueInput<AudioSource>(nameof(audioSourceInput), null);
            Succession(input, output);
            Succession(input, done);
            Requirement(audioSourceInput, input);
        }

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            // Reference from EventUnit stop listening.
            // Can't copy the script from EventUnit.Uninstantiate because it's private.
            var stack = instance.ToStackPooled();
            var data = stack.GetElementData<Data>(this);
            foreach (var coroutine in data.activeCoroutines)
            {
                coroutine.StopCoroutine(false);
            }

            base.Uninstantiate(instance);
        }

        protected void RunDone(Flow flow)
        {
            if (coroutine)
            {
                flow.StartCoroutine(done, flow.stack.GetElementData<Data>(this).activeCoroutines);
            }
            else
            {
                flow.Run(done);
            }
        }

        protected virtual ControlOutput OnInput(Flow flow)
        {
            var audioSource = flow.GetValue<AudioSource>(audioSourceInput);
            if (audioSource == null)
            {
                return output;
            }

            if (fadeDuration <= 0)
            {
                StartWithoutFade(audioSource);
                RunDone(flow);
            }
            else
            {
                // Start coroutine
                var graphReference = flow.stack.ToReference();
                flow.stack.component.StartCoroutine(FadeCoroutine(audioSource, graphReference));
            }

            return output;
        }

        protected virtual void StartWithoutFade(AudioSource audioSource)
        {

        }

        protected virtual IEnumerator FadeCoroutine(AudioSource audioSource, GraphReference graphReference)
        {
            StartWithoutFade(audioSource);
            yield return null;
            RunDone(Flow.New(graphReference));
        }

        public IGraphElementData CreateData()
        {
            return new Data();
        }
    }
}

#endif