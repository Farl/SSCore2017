#if USE_VISUAL_SCRIPTING
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

namespace SS
{
    [UnitCategory("SS")]
    [UnitTitle("Request Dialog")]
    [UnitShortTitle("Request Dialog")]
    public class RequestDialogUnit : Unit, IGraphElementWithData
    {
        public class Data : IGraphElementData
        {
            public HashSet<Flow> activeCoroutines = new HashSet<Flow>();
            public int handle = -1;
            public int buttonIndex = -1;
        }
        
        // Inputs
        [DoNotSerialize, PortLabelHidden] public ControlInput input;
        [DoNotSerialize, PortLabel("Abort")] public ControlInput abortInput;
        [DoNotSerialize, PortLabel("ID")] public ValueInput textIDInput;
        [DoNotSerialize, PortLabel("Type")] public ValueInput dialogTypeInput;
        [DoNotSerialize, PortLabel("Button Texts")] public ValueInput buttonTextsInput;
        [DoNotSerialize, PortLabel("Data")] public ValueInput additionalDataInput;

        [Serialize, Inspectable] public List<string> buttonTextIDs = new List<string>();
        [Serialize, Inspectable] public bool autoClose = false;
        [Serialize, Inspectable] public bool delayButton = false;

        // Outputs
        [DoNotSerialize, PortLabelHidden] public ControlOutput output;
        [DoNotSerialize, PortLabel("On Show")] public ControlOutput onShow;
        [DoNotSerialize, PortLabel("On Hide")] public ControlOutput onHide;
        [DoNotSerialize] public ValueOutput buttonIndexOutput;

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
            abortInput = ControlInput(nameof(abortInput), OnAbort);
            dialogTypeInput = ValueInput<string>(nameof(dialogTypeInput), string.Empty);
            textIDInput = ValueInput<string>(nameof(textIDInput), null);
            buttonTextsInput = ValueInput<List<string>>(nameof(buttonTextsInput));
            additionalDataInput = ValueInput<object>(nameof(additionalDataInput), null);

            output = ControlOutput(nameof(output));
            onShow = ControlOutput(nameof(onShow));
            onHide = ControlOutput(nameof(onHide));
            buttonIndexOutput = ValueOutput<int>(nameof(buttonIndexOutput));

            Succession(input, output);
            Succession(input, onShow);
            Succession(input, onHide);
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

        protected virtual void RunOnHide(Flow flow, int buttonIndex)
        {
            var data = flow.stack.GetElementData<Data>(this);
            flow.SetValue(buttonIndexOutput, buttonIndex);
            if (coroutine)
            {
                flow.StartCoroutine(onHide, data.activeCoroutines);
            }
            else
            {
                flow.Run(onHide);
            }
        }

        protected virtual void RunOnShow(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);
            if (coroutine)
            {
                flow.StartCoroutine(onShow, data.activeCoroutines);
            }
            else
            {
                flow.Run(onShow);
            }
        }

        protected virtual ControlOutput OnInput(Flow flow)
        {
            var graphReference = flow.stack.ToReference();
            
            List<string> texts = new List<string>();
            
            // Add from array
            for (int i = 0; i < buttonTextIDs.Count; i++)
            {
                texts.Add(buttonTextIDs[i]);
            }

            if (buttonTextsInput.hasValidConnection)
            {
                var buttonTexts = flow.GetValue<List<string>>(buttonTextsInput);
                if (buttonTexts != null)
                {
                    foreach (var buttonText in buttonTexts)
                    {
                        texts.Add(buttonText);
                    }
                }
            }

            var buttonSetups = new DialogButtonSetup[texts.Count];
            for (int i = 0; i < texts.Count; i++)
            {
                var index = i;
                buttonSetups[i] = new DialogButtonSetup
                {
                    buttonText = texts[i],
                    onButton = () =>
                    {
                        Data data = graphReference.ToStackPooled().GetElementData<Data>(this);
                        data.buttonIndex = index;
                    }
                };
            }

            var requestData = new DialogRequestData
            {
                typeName = flow.GetValue<string>(dialogTypeInput),
                titleID = $"{flow.GetValue<string>(textIDInput)}_Title",
                descID = flow.GetValue<string>(textIDInput),
                soundEventID = flow.GetValue<string>(textIDInput),
                buttonSetup = buttonSetups,
                autoClose = autoClose,
                delayButton = delayButton,
                onShow = requestData =>
                {
                    RunOnShow(Flow.New(graphReference));
                },
                onHide = (requestData, keepRequest) =>
                {
                    Data data2 = graphReference.ToStackPooled().GetElementData<Data>(this);
                    RunOnHide(Flow.New(graphReference), data2.buttonIndex);
                }
            };
            if (additionalDataInput.hasValidConnection)
            {
                requestData.additionalData = new object[] { flow.GetValue<object>(additionalDataInput) };
            }
            var handle = DialogUI.Request(requestData);
            var data = flow.stack.GetElementData<Data>(this);
            data.buttonIndex = -1;
            data.handle = handle;
            return output;
        }

        protected virtual ControlOutput OnAbort(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);
            DialogUI.Close(data.handle);
            return null;
        }

        public IGraphElementData CreateData()
        {
            return new Data();
        }
    }
}
#endif