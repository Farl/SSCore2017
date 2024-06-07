#if USE_VISUAL_SCRIPTING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SS
{
    [UnitCategory("SS")]
    [UnitTitle("Multi-Gate")]
    [UnitShortTitle("MultiGate")]
    public class MultiGate : Unit, IGraphElementWithData
    {
        #region Unit Interface
        [UnitHeaderInspectable("Count")] public int gateCount = 2;

        [DoNotSerialize, PortLabelHidden] public ControlInput Input;
        [DoNotSerialize, PortLabel("Reset")] public ControlInput Reset;

        [DoNotSerialize, PortLabel("Is Random")] public ValueInput IsRandom;
        [DoNotSerialize, PortLabel("Loop")] public ValueInput Loop;
        [DoNotSerialize, PortLabel("Start Index")] public ValueInput StartIndex;

        [DoNotSerialize, PortLabelHidden] public ControlOutput Output;

        private List<ControlOutput> OutputGates = new List<ControlOutput>();

        [DoNotSerialize, PortLabel("Index")] public ValueOutput Index;
        #endregion

        #region Private

        private class Data : IGraphElementData
        {
            public int currIndex = 0;
        }

        #endregion

        protected override void Definition()
        {
            Input = ControlInput(nameof(Input), OnInput);
            Reset = ControlInput(nameof(Reset), OnReset);

            IsRandom = ValueInput<bool>(nameof(IsRandom), false);
            Loop = ValueInput<bool>(nameof(Loop), false);
            StartIndex = ValueInput<int>(nameof(StartIndex), -1);

            Output = ControlOutput(nameof(Output));

            Index = ValueOutput<int>(nameof(Index));

            gateCount = Mathf.Max(0, gateCount);
            for (var i = 0; i < gateCount; i++)
            {
                var controlOutput = ControlOutput($"Out {i}");
                OutputGates.Add(controlOutput);
                Succession(Input, controlOutput);
            }

            Succession(Input, Output);
            Requirement(IsRandom, Input);
            Requirement(Loop, Input);
            Requirement(StartIndex, Input);
        }

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            base.Uninstantiate(instance);
        }

        private ControlOutput OnInput(Flow flow)
        {
            var stack = flow.stack;
            if (stack == null)
                return null;

            // Get current index
            var data = stack.GetElementData<Data>(this);
            if (data == null)
                return null;

            var currIndex = data.currIndex;

            // Start index
            if (currIndex < 0)
            {
                var startIndex = flow.GetValue<int>(StartIndex);
                if (startIndex < 0)
                    startIndex = 0;
                currIndex = startIndex;
            }

            // Random
            var isRandom = flow.GetValue<bool>(IsRandom);
            if (isRandom)
            {
                var random = Random.Range(0, gateCount);
                currIndex = random;
            }

            // Loop
            if (flow.GetValue<bool>(Loop) && gateCount > 0)
            {
                currIndex %= gateCount;
            }

            // Set current index
            if (isRandom)
            {
                data.currIndex = currIndex;
            }
            else
            {
                data.currIndex = currIndex + 1;
            }

            // Output
            if (currIndex >= 0 && currIndex < OutputGates.Count)
            {
                var graphRef = flow.stack.ToReference();
                var newFlow = Flow.New(graphRef);
                newFlow.Run(OutputGates[currIndex]);
            }

            flow.SetValue(Index, currIndex);
            return Output;
        }

        private ControlOutput OnReset(Flow flow)
        {
            // Set current index
            var startIndex = flow.GetValue<int>(StartIndex);
            if (startIndex < 0)
                startIndex = 0;
            var data = flow.stack.GetElementData<Data>(this);
            data.currIndex = startIndex;

            return null;
        }

        public IGraphElementData CreateData()
        {
            return new Data();
        }
    }
}

#endif