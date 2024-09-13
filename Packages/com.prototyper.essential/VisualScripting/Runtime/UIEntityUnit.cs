#if USE_VISUAL_SCRIPTING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SS
{

    [UnitCategory("SS")]
    [UnitTitle("UIEntity Action")]
    [UnitShortTitle("UIEntity Action")]
    public class UIEntityUnit : Unit, IGraphElementWithData
    {
        public enum UIEntityAction
        {
            Show,
            Hide
        }
        public class Data : IGraphElementData
        {
            public HashSet<Flow> activeCoroutines = new HashSet<Flow>();
        }

        [DoNotSerialize, PortLabelHidden] public ControlInput input;

        [DoNotSerialize, PortLabelHidden] public ValueInput actionInput;
        [DoNotSerialize, PortLabelHidden] public ValueInput uiNameInput;


        [SerializeAs(nameof(argumentCount))]
        private int _argumentCount;

        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Arguments")]
        public int argumentCount
        {
            get => _argumentCount;
            set => _argumentCount = Mathf.Clamp(value, 0, 10);
        }
        [DoNotSerialize]
        public List<ValueInput> argumentPorts { get; } = new List<ValueInput>();

        [DoNotSerialize, PortLabelHidden] public ControlOutput output;
        [DoNotSerialize, PortLabel("On Closed")] public ControlOutput onClosed;

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
            onClosed = ControlOutput(nameof(onClosed));

            uiNameInput = ValueInput<string>(nameof(uiNameInput), null);
            actionInput = ValueInput<UIEntityAction>(nameof(actionInput), UIEntityAction.Show);
            for (int i = 0; i < argumentCount; i++)
            {
                var argument = ValueInput<object>($"arg_{i}");
                argumentPorts.Add(argument);
            }
            Succession(input, output);
            Succession(input, onClosed);
            Requirement(uiNameInput, input);
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

        protected virtual void RunOnClosed(Flow flow)
        {
            if (coroutine)
            {
                flow.StartCoroutine(onClosed, flow.stack.GetElementData<Data>(this).activeCoroutines);
            }
            else
            {
                flow.Run(onClosed);
            }
        }

        protected virtual ControlOutput OnInput(Flow flow)
        {
            var graphReference = flow.stack.ToReference();
            var uiName = flow.GetValue<string>(uiNameInput);
            var uiEntity = UIManager.GetUIEntity(uiName);
            if (uiEntity == null)
            {
                return output;
            }

            void CallOnClose()
            {
                RunOnClosed(Flow.New(graphReference));
                if (uiEntity != null)
                {
                    uiEntity.OnHideComplete -= CallOnClose;
                }
            }

            if (onClosed.hasValidConnection)
                uiEntity.OnHideComplete += CallOnClose;

            var args = new List<object>();
            for (int i = 0; i < argumentPorts.Count; i++)
            {
                if (argumentPorts[i].hasValidConnection)
                    args.Add(flow.GetValue<object>(argumentPorts[i]));
                else
                    args.Add(null);
            }

            var action = flow.GetValue<UIEntityAction>(actionInput);
            if (action == UIEntityAction.Show)
            {
                uiEntity.Show(args.ToArray());
            }
            else
            {
                uiEntity.Hide(args.ToArray());
            }
            return output;
        }
        public IGraphElementData CreateData()
        {
            return new Data();
        }
    }
}
#endif