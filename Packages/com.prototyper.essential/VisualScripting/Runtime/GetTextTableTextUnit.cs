#if USE_VISUAL_SCRIPTING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SS
{
    [UnitCategory("SS")]
    [UnitTitle("Get TextTable Text")]
    [UnitShortTitle("Get TextTable Text")]
    public class GetTextTableTextUnit : Unit
    {
        [DoNotSerialize, PortLabel("Text ID")] public ValueInput textIDInput;
        [DoNotSerialize, PortLabel("Text")] public ValueOutput output;

        protected override void Definition()
        {
            textIDInput = ValueInput<string>(nameof(textIDInput), null);
            output = ValueOutput<string>(nameof(output), (flow) =>
            {
                var name = flow.GetValue<string>(textIDInput);
                if (TextTable.TryGetText(name, out var text))
                {
                    return text;
                }
                else
                {
                    return null;
                }
            });
            Requirement(textIDInput, output);
        }
    }
}
#endif